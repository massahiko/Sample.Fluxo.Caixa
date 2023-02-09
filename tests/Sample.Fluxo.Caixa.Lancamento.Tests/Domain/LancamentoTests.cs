using Sample.Fluxo.Caixa.Core.DomainObjects;
using System;
using Xunit;

namespace Sample.Fluxo.Caixa.Lancamento.Tests.Domain
{
    public class LancamentoTests
    {
        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", "86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public void Lancamento_CriarEntidade_DeveCriarComSucesso(
            Guid lancamentoId,
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange & Act
            var conta = new Lancamento.Domain.Lancamento(lancamentoId, contaId, valor, dataEscrituracao);

            // Assert
            Assert.NotNull(conta);
        }

        [Fact]
        public void Lancamento_Validar_ValidacoesDevemRetornarExceptions()
        {
            //Arrange & Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                new Lancamento.Domain.Lancamento(Guid.Empty, Guid.Empty, 100, DateTime.Now)
            );

            Assert.Equal("O campo ContaId do lançamento não pode estar vazio", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Lancamento.Domain.Lancamento(Guid.Empty, Guid.NewGuid(), 0, DateTime.Now)
            );

            Assert.Equal("O campo Valor do lançamento deve possuir valor maior que 0", ex.Message);

            ex = Assert.Throws<DomainException>(() =>
                new Lancamento.Domain.Lancamento(Guid.Empty, Guid.NewGuid(), 100, DateTime.MinValue)
            );

            Assert.Equal("O campo DataEscrituracao do lançamento não pode estar vazio", ex.Message);
        }
    }
}
