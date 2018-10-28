using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageServer
{
    public class RequestHandler : IRequestHandler
    {
        private static HttpClient _httpClient = new HttpClient();

        private ILogger _logger;
        private ISettings _settings;
        private RateLimiter _rateLimiter;
        private UrlParser _urlParser;

        public RequestHandler(ISettings settings) : this(settings, new Logger(settings.LoggerSettings))
        {
        }

        public RequestHandler(ISettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
            _rateLimiter = new RateLimiter(_settings.RateLimiterSettings, _logger);
            _urlParser = new UrlParser(_settings.SourceRoot, _settings.DestinationRoot);
        }

        public async Task<IRequestHandlerResult> HandleRequest(Uri url, string ipAddress)
        {
            if (_rateLimiter.IsBlacklisted(ipAddress))
            {
                _logger.LogInfo($"Denied request from blacklisted ip: {ipAddress}");
                return new RequestHandlerResult(ResultStatus.Forbidden);
            }

            if (_rateLimiter.IsLimitExceeded(ipAddress))
            {
                _logger.LogInfo($"Denied request because limit was exceeded for ip: {ipAddress}");
                return new RequestHandlerResult(ResultStatus.TooManyRequests);
            }

            try
            {
                UrlParserResult urlParserResult = _urlParser.Parse(url);
                byte[] sourceBytes = await GetSourceImage(urlParserResult.Source);
                _rateLimiter.RegisterRequest(ipAddress, sourceBytes.LongLength);
                
                await FileAccessor.RunAction(urlParserResult.Destination, () => ResizeSourceImage(sourceBytes, urlParserResult));

                _logger.LogInfo($"Created image at: {urlParserResult.Destination}");

                string contentType = "image/" + urlParserResult.ImageType.ToString().ToLower();
                FileInfo fileInfo = new FileInfo(urlParserResult.Destination);
                return new RequestHandlerResult(ResultStatus.OK, new ImageInfo(fileInfo.Length, contentType, urlParserResult.Destination));
            }
            catch (UrlParserException)
            {
                _logger.LogWarning($"Could not parse url: {url}");
                return new RequestHandlerResult(ResultStatus.BadRequest);
            }
            catch (FileNotFoundException e)
            {
                _logger.LogWarning($"Could not find source: {e.FileName}");
                return new RequestHandlerResult(ResultStatus.FileNotFound);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Exception was thrown trying to fetch remote source: {e.ToString()}");
                return new RequestHandlerResult(ResultStatus.BadGateway);
            }
            catch (Exception e)
            {
                _logger.LogError($"An unhandled exception occurred: {e.ToString()}");
                return new RequestHandlerResult(ResultStatus.ServerError);
            }
        }

        private async Task<byte[]> GetSourceImage(string location)
        {
            if (Regex.IsMatch(location, "^https?://", RegexOptions.IgnoreCase))
            {
                byte[] bytes = await _httpClient.GetByteArrayAsync(new Uri(location));
                return bytes;
            }
            else
            {
                byte[] bytes = File.ReadAllBytes(location);
                return bytes;
            }
        }

        private async Task ResizeSourceImage(byte[] sourceBytes, UrlParserResult urlParserResult)
        {
            string preProcessedFile = null;
            string postProcessorCommand = GetPostProcessorCommand(urlParserResult);
            if (!string.IsNullOrEmpty(postProcessorCommand))
            {
                preProcessedFile = Regex.Replace(urlParserResult.Destination, @"\.(gif|jpe?g|png)$", new MatchEvaluator((match) => { return ".tmp" + match.Value; }), RegexOptions.IgnoreCase);
            }

            ImageFit.IEncoder encoder = GetEncoder(urlParserResult);

            Action<byte[], string, int, int, ImageFit.IEncoder> action;
            ResizeMethod resizeMethod = urlParserResult.ResizeMethod ?? ResizeMethod.ScaleDown;
            switch (resizeMethod)
            {
                case ResizeMethod.Contain: action = ImageFit.Image.Contain; break;
                case ResizeMethod.Cover: action = ImageFit.Image.Cover; break;
                case ResizeMethod.Fill: action = ImageFit.Image.Fill; break;
                case ResizeMethod.ScaleDown: action = ImageFit.Image.ScaleDown; break;
                default: throw new NotSupportedException();
            }
            action(sourceBytes, preProcessedFile ?? urlParserResult.Destination, urlParserResult.Width, urlParserResult.Height, encoder);


            if (!string.IsNullOrEmpty(postProcessorCommand))
            {
                string command = postProcessorCommand
                    .Replace("{inputfile}", preProcessedFile)
                    .Replace("{outputfile}", urlParserResult.Destination);

                SimpleShell.ICommandResult commandResult = await SimpleShell.Command.Run(command);

                File.Delete(preProcessedFile);

                File.Delete(urlParserResult.Destination + ".bak"); // OptiPng likes creating .bak files...

                if (!string.IsNullOrEmpty(commandResult.StandardOutput))
                {
                    _logger.LogInfo(commandResult.StandardOutput);
                }

                if (!string.IsNullOrEmpty(commandResult.StandardError))
                {
                    _logger.LogInfo(commandResult.StandardError);
                }
                
                if (commandResult.ErrorType != SimpleShell.CommandErrorType.None)
                {
                    File.Delete(urlParserResult.Destination);
                    throw new Exception($"Error running post-processor command: {postProcessorCommand}");
                }
            }
        }

        private string GetPostProcessorCommand(UrlParserResult urlParserResult)
        {
            bool forcedOn = urlParserResult.PostProcess.HasValue && urlParserResult.PostProcess.Value == true;
            bool forcedOff = urlParserResult.PostProcess.HasValue && urlParserResult.PostProcess.Value == false;

            Func<IPostProcessorSettings, string> GetCommand = (settings) => 
            {
                string cmd = string.Empty;

                if (forcedOn || (settings.PostProcessorEnabled && !forcedOff))
                {
                    cmd = settings.PostProcessorCommand;
                    if (string.IsNullOrEmpty(cmd))
                    {
                        _logger.LogWarning($"Post-processing requested but no post-processor command specified.");
                    }
                }

                return cmd;
            };
            

            string command = string.Empty;

            if (!forcedOff)
            {
                switch (urlParserResult.ImageType)
                {
                    case ImageType.Gif:
                        command = GetCommand(_settings.GifSettings);
                        break;

                    case ImageType.Jpeg:
                        command = GetCommand(_settings.JpegSettings);
                        break;

                    case ImageType.Png:
                        command = GetCommand(_settings.PngSettings);
                        break;
                }
            }

            return command;
        }

        private ImageFit.IEncoder GetEncoder(UrlParserResult urlParserResult)
        {
            switch (urlParserResult.ImageType)
            {
                case ImageType.Gif:
                    return new ImageFit.GifEncoder();

                case ImageType.Jpeg:
                    int quality = urlParserResult.Quality.HasValue ? urlParserResult.Quality.Value : _settings.JpegSettings.Quality;
                    return new ImageFit.JpegEncoder(quality);

                case ImageType.Png:
                    int compressionLevel = urlParserResult.Compression.HasValue ? urlParserResult.Compression.Value : _settings.PngSettings.CompressionLevel;
                    return new ImageFit.PngEncoder(compressionLevel);

                default:
                    throw new NotSupportedException();
            }
        }

        private void HandleOptiPngBugs()
        {
        }
    }
}
