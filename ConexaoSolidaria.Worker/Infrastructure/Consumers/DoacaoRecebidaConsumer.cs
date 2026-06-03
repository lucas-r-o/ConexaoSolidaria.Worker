using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Infrastructure.Persistence;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Consumers
{
    internal class DoacaoRecebidaConsumer(ICampanhaRepository repository) : IConsumer<DoacaoRecebidaEvent>
    {
        private readonly ICampanhaRepository _repository = repository;

        public async Task Consume(ConsumeContext<DoacaoRecebidaEvent> context)
        {
            var campanhaId = context.Message.CampanhaId;

            if (!_repository.TryGet(campanhaId, out var campanha))
            {
                // TODO: Validar um caminho melhor
                throw new Exception($"Campanha {campanhaId} não encontrada");
            }

            campanha!.UpdateArrecadacao(context.Message.Valor);

            await _repository.UpdateAsync(campanha);
        }
    }
}
