using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConexaoSolidaria.Worker.Infrastructure.Extensions
{
    public static class ContextExtensions
    {
        private const string WORKER_CONNECTIONSTRING_NAME = "Worker";

        public static void AddDataBaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(WORKER_CONNECTIONSTRING_NAME);

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(nameof(connectionString));

            services.AddDbContext<WorkerDbContext>(options => options.UseNpgsql(connectionString));
        }

        public static void AddRepository(this IServiceCollection services) => services.AddScoped<ICampanhaRepository, CampanhaRepository>();
    }
}
