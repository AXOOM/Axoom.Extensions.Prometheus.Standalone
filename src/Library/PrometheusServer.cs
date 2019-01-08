using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Axoom.Extensions.Prometheus.Standalone
{
    /// <summary>
    /// A simple service for providing Prometheus metrics using an embedded HTTP server.
    /// </summary>
    public class PrometheusServer : IHostedService
    {
        private readonly string _prefix;
        private readonly HttpListener _listener;
        private readonly IExposable _metrics;
        private readonly ILogger<PrometheusServer> _logger;

        public PrometheusServer(IOptions<PrometheusServerOptions> options, IExposable metrics, ILogger<PrometheusServer> logger)
        {
            _prefix = $"http://*:{options.Value.Port}/";
            _listener = new HttpListener {Prefixes = {_prefix}};
            _metrics = metrics;
            _logger = logger;
        }

        /// <summary>
        /// Starts exposing metrics.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listener.Start();
            }
            catch (HttpListenerException ex) when (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.LogWarning(ex, "Unable to open port to expose metrics. Please run the following as admin and then retry:\nnetsh http add urlacl {0} user={1}\\{2}",
                    _prefix, Environment.GetEnvironmentVariable("USERDOMAIN"), Environment.GetEnvironmentVariable("USERNAME"));
                return Task.CompletedTask;
            }

            BeginContext();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops exposing metrics.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listener.Abort();

            return Task.CompletedTask;
        }

        private void BeginContext()
        {
            try
            {
                _listener.BeginGetContext(ListenerCallback, _listener);
            }
            catch (ObjectDisposedException)
            {
                // Do not throw exception on shutdown
            }
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var context = _listener.EndGetContext(result);
                if (context.Request.HttpMethod == "GET")
                {
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Content-Type", "text/plain");
                    await _metrics.Expose(context.Response.OutputStream, ExposeOptions.Default);
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
                _logger.LogWarning(ex, "HTTP problem error while providing metric");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while providing metric");
            }
            finally
            {
                BeginContext();
            }
        }
    }
}
