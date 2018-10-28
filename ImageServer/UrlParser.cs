using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ImageServer
{
    internal class UrlParser
    {
        private string _sourceRoot;
        private ILocation _destinationRoot;


        public UrlParser(string sourceRoot, ILocation destinationRoot)
        {
            _sourceRoot = sourceRoot.TrimEnd('/','\\');
            _destinationRoot = new Location(new Uri(destinationRoot.AbsoluteUrl.OriginalString.TrimEnd('/')), destinationRoot.AbsolutePath.TrimEnd('/', '\\'));
        }


        public UrlParserResult Parse(Uri url)
        {
            string pattern = @"^
                                (?<location>.*)
                                /
                                (?<modifier>
                                    (?<width>\d+)x(?<height>\d+)
                                    (
                                        -(?<resize>contain|cover|fill|scaledown)
                                        |
                                        -(q(?<quality>\d+))
                                        |
                                        -(c(?<compression>\d+))
                                        |
                                        -(?<postprocess>n?pp)
                                    )*
                                )
                                /
                                (?<file>[^/]+\.(?<extension>gif|jpe?g|png))
                                $";


            Match match = Regex.Match(url.GetLeftPart(UriPartial.Path), pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (!match.Success)
            {
                throw new UrlParserException($"The URL {url.GetLeftPart(UriPartial.Path)} is not a valid.");
            }

            string location = match.Groups["location"].Value;
            string modifier = match.Groups["modifier"].Value;
            string file = match.Groups["file"].Value;
            string extension = match.Groups["extension"].Value;

            string root = _destinationRoot.AbsoluteUrl.OriginalString;
            string dirs = string.Empty;
            if (location.StartsWith(root, StringComparison.InvariantCultureIgnoreCase))
            {
                dirs = location.Replace(root, "").Trim('/');
            }
            else
            {
                throw new UrlParserException($"The requested location {location} is not below the root location {root}.");
            }

            UrlParserResult result = new UrlParserResult()
            {
                Source = GetSource(dirs, file),
                Destination = GetDestination(dirs, modifier, file),
                ImageType = GetImageType(extension),
                Width = Convert.ToInt32(match.Groups["width"].Value),
                Height = Convert.ToInt32(match.Groups["height"].Value),
                ResizeMethod = GetValueOrNull<ResizeMethod>(match.Groups["resize"].Value),
                Quality = GetValueOrNull<int>(match.Groups["quality"].Value),
                Compression = GetValueOrNull<int>(match.Groups["compression"].Value)
            };
            
            string postProcessValue = match.Groups["postprocess"].Value;
            if (!string.IsNullOrEmpty(postProcessValue))
            {
                result.PostProcess = postProcessValue == "pp";
            }

            return result;
        }


        private T? GetValueOrNull<T>(string value) where T:struct
        {
            T? result = null;

            if (!string.IsNullOrEmpty(value))
            {
                if (typeof(T).IsEnum)
                {
                    T enumValue;
                    if (Enum.TryParse<T>(value, true, out enumValue))
                    {
                        result = enumValue;
                    }
                }
                else
                {
                    result = (T)Convert.ChangeType(value, typeof(T));
                }
            }

            return result;
        }

        

        private string GetSource(string dirs, string file)
        {
            string source;
            if (Regex.IsMatch(_sourceRoot, "^https?://", RegexOptions.IgnoreCase))
            {
                string sourcePath = _sourceRoot.TrimEnd('/');
                if (!string.IsNullOrEmpty(dirs))
                {
                    sourcePath += "/" + dirs;
                }
                source = $"{sourcePath}/{file}";
            }
            else
            {
                char separator = GetDirectorySeparator();
                if (separator == '\\')
                {
                    dirs = dirs.Replace('/', '\\');
                }

                List<string> parts = new List<string>();
                parts.Add(_sourceRoot);
                if (!string.IsNullOrEmpty(dirs))
                {
                    parts.Add(dirs);
                }
                parts.Add(file);

                source = string.Join(separator.ToString(), parts);
            }
            return source;
        }
            
        private string GetDestination(string dirs, string modifier, string file)
        {
            char separator = GetDirectorySeparator();
            if (separator == '\\')
            {
                dirs = dirs.Replace('/', '\\');
            }

            List<string> parts = new List<string>();
            parts.Add(_destinationRoot.AbsolutePath);
            if (!string.IsNullOrEmpty(dirs))
            {
                parts.Add(dirs);
            }
            parts.Add(modifier);
            parts.Add(file);

            string destination = string.Join(separator.ToString(), parts);
            return destination;
        }

        private ImageType GetImageType(string extension)
        {
            switch (extension.ToLower())
            {
                case "gif":
                    return ImageType.Gif;

                case "jpg":
                case "jpeg":
                    return ImageType.Jpeg;

                case "png":
                    return ImageType.Png;

                default:
                    throw new UrlParserException($"The extension {extension} is not supported.");
            }
        }

        private char GetDirectorySeparator()
        {
            char separator = Path.DirectorySeparatorChar;
            if (_destinationRoot.AbsolutePath.Contains("/"))
            {
                separator = '/';
            }
            else if (_destinationRoot.AbsolutePath.Contains("\\"))
            {
                separator = '\\';
            }
            return separator;
        }
    }
}
