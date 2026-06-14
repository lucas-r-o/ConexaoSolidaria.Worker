using Prometheus;

namespace ConexaoSolidaria.Worker.Infrastructure.Extensions
{
    public static class MetricsExtensions
    {
        public static void AddMetrics(this WebApplication builder)
        {
            builder.UseHttpMetrics();
            builder.MapMetrics();
        }
    }
}
