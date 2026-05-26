using ConexaoSolidaria.Worker.Events;
using MassTransit;

namespace ConexaoSolidaria.Worker.Infrastructure.Consumers
{
    internal class DoacaoRecebidaConsumer : IConsumer<DoacaoRecebidaEvent>
    {
        public Task Consume(ConsumeContext<DoacaoRecebidaEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
