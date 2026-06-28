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
                var resultMessage = "Doação realizada";

                if (!campanha!.Ativo)
                    resultMessage = "Doação não realizada, campanha inativa";
                else
                {
                    campanha!.UpdateArrecadacao(message.Valor);
                    await _repository.UpdateAsync(campanha, context.CancellationToken);
                }

                await _publish.Publish(new DoacaoResultadoEvent
                {
                    CampanhaId = campanha.CampanhaId,
                    DoadorId = message.DoadorId,
                    Motivo = resultMessage,
                    OcorridoEm = DateTimeOffset.UtcNow
                }, context.CancellationToken);
            }
        }
    }
}
