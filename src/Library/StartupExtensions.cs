using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexogen.Libraries.Metrics;
using Nexogen.Libraries.Metrics.Prometheus;

namespace Axoom.Extensions.Prometheus.Standalone
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPrometheusServer(this IServiceCollection services, IConfiguration configuration = null)
        {
            var metrics = new PrometheusMetrics();
            return services.Configure<PrometheusServerOptions>((configuration ?? new ConfigurationBuilder().Build()).GetSection("PrometheusServer"))
                           .AddSingleton<IMetrics>(metrics)
                           .AddSingleton<IExposable>(metrics)
                           .AddSingleton<PrometheusServerOptions>()
                           .AddSingleton<PrometheusServer>();
        }

        public static void UsePrometheusServer(this IServiceProvider provider)
            => provider.GetRequiredService<PrometheusServer>();
    }
}
