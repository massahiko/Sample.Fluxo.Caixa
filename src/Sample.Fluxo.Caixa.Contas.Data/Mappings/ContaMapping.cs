using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;

namespace Sample.Fluxo.Caixa.PlanoContas.Data.Mappings
{
    public class ContaMapping : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Descricao)
                .IsRequired()
                .HasColumnType("varchar(250)");

            builder.Property(c => c.Descricao)
                .IsRequired()
                .HasColumnType("varchar(500)");

            builder.ToTable("Contas");

            builder.HasData(
                new Conta("Saldo Inicial", true, ContaTipo.SaldoInicial, DateTime.Now),
                new Conta("Vendas", true, ContaTipo.Receita, DateTime.Now),
                new Conta("Juros", true, ContaTipo.Receita, DateTime.Now),
                new Conta("Financiamento", true, ContaTipo.Receita, DateTime.Now),
                new Conta("Entrada", true, ContaTipo.Receita, DateTime.Now),
                new Conta("Fornecedores", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Materiais", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Retiradas sócios", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Folha de pagamento", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Aluguel", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Energia elétrica", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Telefone", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Serviços contabilidade", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Empréstimos", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Financiamentos", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Outros pagamentos", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Investimentos", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Impostos", true, ContaTipo.Despesa, DateTime.Now),
                new Conta("Advogado", true, ContaTipo.Despesa, DateTime.Now));
        }
    }
}
