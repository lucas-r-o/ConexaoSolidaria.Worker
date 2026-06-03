using ConexaoSolidaria.Worker.Models;
using Microsoft.EntityFrameworkCore;

namespace ConexaoSolidaria.Worker.Infrastructure.Persistence
{
    internal class WorkerDbContext : DbContext
    {
        public WorkerDbContext(DbContextOptions<WorkerDbContext> context) : base(context) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkerDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ArrecadacaoCampanha> ArrecadacaoCampanha { get; set; }
    }
}
