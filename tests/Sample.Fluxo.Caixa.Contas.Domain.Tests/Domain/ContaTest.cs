using Sample.Fluxo.Caixa.Core.DomainObjects;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using Xunit;

namespace Sample.Fluxo.Caixa.PlanoContas.Tests.Domain
{
    public class ContaTest
    {
        [Theory]
        [InlineData("Criar", ContaTipo.SaldoInicial, "2023-01-01")]
        public void Conta_CriarEntidade_DeveCriarComSucesso(
            string descricao,
            ContaTipo contaTipo,
            DateTime dataCadastro)
        {
            // Arrange & Act
            var conta = new Conta(descricao, true, contaTipo, dataCadastro);

            // Assert
            Assert.NotNull(conta);
        }

        [Fact]
        public void Conta_Validar_ValidacoesDevemRetornarExceptions()
        {
            //Arrange & Act & Assert
            var ex = Assert.Throws<DomainException>(() =>
                new Conta(string.Empty, false, ContaTipo.Despesa, DateTime.Now)
            );

            Assert.Equal("O campo Descrição da Conta não pode estar vazio", ex.Message);
        }
    }
}
