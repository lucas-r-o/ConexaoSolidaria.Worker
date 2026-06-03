using ConexaoSolidaria.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace ConexaoSolidaria.Worker.Infrastructure.Persistence
{
    internal class CampanhaRepository(WorkerDbContext db) : ICampanhaRepository
    {
        private readonly WorkerDbContext _db = db;
        public bool TryGet(Guid campanhaId, out ArrecadacaoCampanha? campanha)
        {
            campanha = _db.ArrecadacaoCampanha.FirstOrDefault(p => p.CampanhaId == campanhaId);

            return campanha != null;
        }

        public async Task<bool> CreateAsync(Guid campanhaId, long valor)
        {
            var campanha = new ArrecadacaoCampanha() { CampanhaId = campanhaId };
            campanha.UpdateArrecadacao();

            _ = await _db.ArrecadacaoCampanha.AddAsync(campanha);

            var result = await _db.SaveChangesAsync();

            return result > 0;
        }
        public async Task UpdateAsync(ArrecadacaoCampanha campanha)
        {
            _ = _db.Update(campanha);

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ArrecadacaoCampanha>> GetAllCampanhas(bool apenasAtivos = false)
        {
            var campanhas = _db.ArrecadacaoCampanha.AsQueryable();

            if (apenasAtivos)
                campanhas = campanhas.Where(p => p.Ativo);

            return await campanhas.ToListAsync();
        }
    }
}
