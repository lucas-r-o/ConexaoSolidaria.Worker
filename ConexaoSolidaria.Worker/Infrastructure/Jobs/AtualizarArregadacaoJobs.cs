using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Jobs
{
    //public class AtualizarArregadacaoJobs(IPublishEndpoint publish, ICampanhaRepository repository) : BackgroundService
    public class AtualizarArregadacaoJobs(IServiceScopeFactory scopeFactory) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                await using var scope = scopeFactory.CreateAsyncScope();

                var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var repository = scope.ServiceProvider.GetRequiredService<ICampanhaRepository>();

                var campanhas = await repository.GetAllCampanhas(true);

                if (campanhas == null || !campanhas.Any()) continue; // ← era return, causaria bug

                var tasks = campanhas
                    .Select(c => publish.Publish(
                        new ArrecadacaoAtualizadaEvent(c.CampanhaId, c.TotalArrecadado, DateTime.Now),
                        stoppingToken));

                await Task.WhenAll(tasks);
            }
        }
    }
}
