namespace Axoom.Extensions.Prometheus.Standalone
{
    /// <summary>
    /// Options Poco for configuration the <see cref="PrometheusServer"/>.
    /// </summary>
    public class PrometheusServerOptions
    {
        /// <summary>
        /// Gets or sets the port number to expose Prometheus metrics on (Default: 5000).
        /// </summary>
        public int Port { get; set; } = 5000;
    }
}
