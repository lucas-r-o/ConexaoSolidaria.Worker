using ConexaoSolidaria.Worker.Models;

namespace ConexaoSolidaria.Worker.Infrastructure.Persistence
{
    public interface ICampanhaRepository
    {
        bool TryGet(Guid campanhaId, out ArrecadacaoCampanha? campanha);
        Task<bool> CreateAsync(Guid campanhaId, long valor);
        Task UpdateAsync(ArrecadacaoCampanha campanha);
        Task<IEnumerable<ArrecadacaoCampanha>> GetAllCampanhas(bool apenasAtivos = false);
    }
}
