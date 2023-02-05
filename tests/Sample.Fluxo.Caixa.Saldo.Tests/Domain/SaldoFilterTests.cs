using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Text;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Domain
{
    public class SaldoFilterTests
    {
        [Fact]
        public void SaldoFilter_Build_RetornaExpressao()
        {
            // Arrange
            var saldoFilter = new SaldoFilter()
            {
                Id = Guid.NewGuid(),
                DataEscrituracao = DateTime.Now,
                DataCadastro = DateTime.Now
            };

            // Act
            var result = saldoFilter.Build();

            // Assert
            var textFilter = new StringBuilder($"x => (((True AndAlso (x.Id == value({typeof(SaldoFilter).FullName}).Id)) AndAlso");
            textFilter.Append($" (Convert(x.DataCadastro, Nullable`1) == value({typeof(SaldoFilter).FullName}).DataCadastro))")
                      .Append($" AndAlso (Convert(x.DataCadastro, Nullable`1) == value({typeof(SaldoFilter).FullName}).DataCadastro))");

            Assert.Equal(textFilter.ToString(), result.ToString());
        }
    }
}
