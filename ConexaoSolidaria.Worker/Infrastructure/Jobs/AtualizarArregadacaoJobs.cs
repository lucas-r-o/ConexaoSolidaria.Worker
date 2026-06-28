using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Jobs
{
    public class AtualizarArregadacaoJobs(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        // TODO: Melhorar job para enviar atualização apenas quando tiver alteração de valor
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                await using var scope = scopeFactory.CreateAsyncScope();

                var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var repository = scope.ServiceProvider.GetRequiredService<ICampanhaRepository>();

                var campanhas = await repository.GetAllCampanhas(true);

                if (campanhas == null || !campanhas.Any()) continue;

                var conquistadas = campanhas.Where(c => c.MetaConquistada);

                foreach (var campanha in conquistadas)
                    campanha.Finish();

                var alteradas = campanhas.Where(c => c.HouveMudanca);

                if (!alteradas.Any()) continue;

                foreach (var campanha in alteradas)
                    campanha.UpdateUltimoTotal();

                await repository.UpdateRangeAsync(campanhas, cancelattionToken);

                var events = campanhas.Select(c => new ArrecadacaoAtualizadaEvent(c.CampanhaId, c.TotalArrecadado, DateTimeOffset.UtcNow));

                await Task.WhenAll(events.Select(e => publish.Publish(e, stoppingToken)));
            }
        }
    }
}
