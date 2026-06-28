using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Consumers
{
    internal class DoacaoRecebidaConsumer(ICampanhaRepository repository, IPublishEndpoint publish) : IConsumer<DoacaoRecebidaEvent>
    {
        private readonly ICampanhaRepository _repository = repository;
        private readonly IPublishEndpoint _publish = publish;

        public async Task Consume(ConsumeContext<DoacaoRecebidaEvent> context)
        {
            var message = context.Message;

            // TODO: pensar melhor a estrutura de criação das campanhas caso não exista
            if (!_repository.TryGet(message.CampanhaId, out var campanha))
                _ = await _repository.CreateAsync(message);
            else
            {
                if (!campanha!.Ativo)
                {
                    await _publish.Publish(new DoacaoRecusadaEvent
                    {
                        CampanhaId = campanha.CampanhaId,
                        DoadorId = message.DoadorId,
                        Motivo = "Campanha inativa",
                        OcorridoEm = DateTimeOffset.UtcNow
                    }, context.CancellationToken);
                }
                else
                {
                    campanha!.UpdateArrecadacao(message.Valor);
                    await _repository.UpdateAsync(campanha, context.CancellationToken);
                }
            }
        }
    }
}
