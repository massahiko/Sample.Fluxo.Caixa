using Microsoft.EntityFrameworkCore;
using Sample.Fluxo.Caixa.Core.Data;
using System.Linq;
using System;
using System.Threading.Tasks;
using Sample.FluxoCaixa.PlanoContas.Domain;
using Sample.Fluxo.Caixa.Core.Messages;

namespace Sample.FluxoCaixa.PlanoContas.Data
{
    public class PlanoContasContext : DbContext, IUnitOfWork
    {
        public PlanoContasContext(DbContextOptions<PlanoContasContext> options)
            : base(options) { }

        public DbSet<Conta> Contas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlanoContasContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return await base.SaveChangesAsync() > 0;
        }
    }
}
