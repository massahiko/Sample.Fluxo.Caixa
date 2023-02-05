using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Text;
using Xunit;

namespace Sample.Fluxo.Caixa.Lancamento.Tests.Domain
{
    public class LancamentoFilterTest
    {
        [Fact]
        public void SaldoFilter_Build_RetornaExpressao()
        {
            // Arrange
            var saldoFilter = new LancamentoFilter()
            {
                Id = Guid.NewGuid(),
                ContaId = Guid.NewGuid(),
                DataEscrituracao = DateTime.Now,
                DataCadastro = DateTime.Now
            };

            // Act
            var result = saldoFilter.Build();

            // Assert
            var textFilter = new StringBuilder($"x => ((((True AndAlso (x.Id == value({typeof(LancamentoFilter).FullName}).Id)) AndAlso");
            textFilter.Append($" (x.ContaId == value({typeof(LancamentoFilter).FullName}).ContaId)) AndAlso")
                      .Append($" (Convert(x.DataCadastro, Nullable`1) == value({typeof(LancamentoFilter).FullName}).DataCadastro)) AndAlso")
                      .Append($" (Convert(x.DataCadastro, Nullable`1) == value({typeof(LancamentoFilter).FullName}).DataCadastro))");

            Assert.Equal(textFilter.ToString(), result.ToString());
        }
    }
}
