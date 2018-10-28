using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;

namespace ImageServer.Demo
{
    public class Startup
    {
        IRequestHandler requestHandler;

        public Startup(IConfiguration configuration)
        {
            // Create a RequestHandler instance based on the settings defined in appsettings.json.
            // This is a .NET Core implementation. 
            // For a .NET Framework implementation, the settings could/should be read from web.config.

            ISettings settings = AppSettingsReader.GetSettings(configuration);
            ILogger logger = new Logger(settings.LoggerSettings);
            requestHandler = new RequestHandler(settings, logger);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // If this is put behind a reverse proxy, like Nginx, make sure to use forwarded headers, to get the users ip for rate-limiting.

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

            // The first hit to a given image-URL will create the image, but once the image has been created
            // subsequent hits to the same image-URL should serve the already created image (static file).

            app.UseStaticFiles();
            
            app.Run(async (context) =>
            {
                string url = context.Request.GetDisplayUrl();
                string ipAddress = context.Connection.RemoteIpAddress.ToString();

                IRequestHandlerResult result = await requestHandler.HandleRequest(new Uri(url), ipAddress);

                if (result.Status == ResultStatus.OK && result.ImageInfo != null)
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = result.ImageInfo.ContentType;
                    context.Response.ContentLength = result.ImageInfo.ByteLength;
                    await context.Response.SendFileAsync(result.ImageInfo.FilePath);
                }
                else
                {
                    context.Response.StatusCode = (int)result.Status;
                }
            });
        }
    }
}
