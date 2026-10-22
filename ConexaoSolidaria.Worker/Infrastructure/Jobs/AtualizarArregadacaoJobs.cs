using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Jobs
{
    public class AtualizarArregadacaoJobs(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancelattionToken)
        {
            while (!cancelattionToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), cancelattionToken);

                await using var scope = scopeFactory.CreateAsyncScope();

                var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var repository = scope.ServiceProvider.GetRequiredService<ICampanhaRepository>();

                var campanhas = await repository.GetAllCampanhas(true);

                if (campanhas == null || !campanhas.Any()) continue;

                var conquistadas = campanhas.Where(c => c.MetaConquistada);

                foreach (var campanha in conquistadas)
                    campanha.Finish();

                var alteradas = campanhas.Where(c => c.HouveMudanca).ToList();

                if (alteradas.Count == 0) continue;

                foreach (var campanha in alteradas)
                    campanha.UpdateUltimoTotal();

                await repository.UpdateRangeAsync(campanhas, cancelattionToken);

                var events = campanhas.Select(c => new ArrecadacaoAtualizadaEvent(c.CampanhaId, c.TotalArrecadado, DateTimeOffset.UtcNow));

                await Task.WhenAll(events.Select(e => publish.Publish(e, cancelattionToken)));
            }
        }
    }
}
