using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sample.Fluxo.Caixa.Lancamento.Data.Mappings
{
    public class LancamentoMapping : IEntityTypeConfiguration<Domain.Lancamento>
    {
        public void Configure(EntityTypeBuilder<Domain.Lancamento> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.DataEscrituracao)
                .IsRequired();

            builder.ToTable("Lancamentos");
        }
    }
}
