namespace ConexaoSolidaria.Worker.Infrastructure.Jobs
{
    public static class JobExtensions
    {
        public static void AddJobs(this IServiceCollection services) => services.AddHostedService<AtualizarArregadacaoJobs>();
    }
}
