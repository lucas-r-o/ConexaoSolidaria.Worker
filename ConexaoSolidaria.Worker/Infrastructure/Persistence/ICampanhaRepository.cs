using ConexaoSolidaria.Events;
using ConexaoSolidaria.Worker.Models;

namespace ConexaoSolidaria.Worker.Infrastructure.Persistence
{
    public interface ICampanhaRepository
    {
        bool TryGet(Guid campanhaId, out ArrecadacaoCampanha? campanha);
        Task<bool> CreateAsync(DoacaoRecebidaEvent doacao);
        Task UpdateAsync(ArrecadacaoCampanha campanha);
        Task UpdateRangeAsync(IEnumerable<ArrecadacaoCampanha> campanhas);
        Task<IEnumerable<ArrecadacaoCampanha>> GetAllCampanhas(bool apenasAtivos = false);
    }
}
