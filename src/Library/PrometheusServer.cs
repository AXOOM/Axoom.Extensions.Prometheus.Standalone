using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Axoom.Extensions.Prometheus.Standalone
{
    /// <summary>
    /// A simple service for providing Prometheus metrics using an embedded HTTP server.
    /// </summary>
    public class PrometheusServer : IDisposable
    {
        private readonly HttpListener _listener;

        /// <summary>
        /// Starts exposing metrics.
        /// </summary>
        public PrometheusServer(IOptions<PrometheusServerOptions> options, ILogger<PrometheusServer> logger, IExposable metrics)
        {
            string prefix = $"http://*:{options.Value.Port}/";

            try
            {
                _listener = new HttpListener {Prefixes = {prefix}};
                _listener.Start();
                _listener.BeginGetContext(ListenerCallback, _listener);
            }
            catch (ObjectDisposedException)
            {
                // Do not throw exception on shutdown
            }
            catch (Exception ex) when (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                logger.LogCritical(ex, "Please run the following as admin and then retry:\nnetsh http add urlacl {0} user={1}\\{2}",
                    prefix, Environment.GetEnvironmentVariable("USERDOMAIN"), Environment.GetEnvironmentVariable("USERNAME"));
                throw;
            }

            async void ListenerCallback(IAsyncResult result)
            {
                try
                {
                    var context = _listener.EndGetContext(result);
                    if (context.Request.HttpMethod == "GET")
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Headers.Add("Content-Type", "text/plain");
                        await metrics.Expose(context.Response.OutputStream, ExposeOptions.Default);
                    }
                    else context.Response.StatusCode = 405; // Method not allowed

                    context.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    // Do not throw exception on shutdown
                }
                catch (HttpListenerException ex)
                {
                    logger.LogWarning(ex, "HTTP problem error while providing metric");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error while providing metric");
                }
                finally
                {
                    _listener.BeginGetContext(ListenerCallback, _listener);
                }
            }
        }

        /// <summary>
        /// Stops exposing metrics.
        /// </summary>
        public void Dispose() => _listener.Abort();
    }
}
