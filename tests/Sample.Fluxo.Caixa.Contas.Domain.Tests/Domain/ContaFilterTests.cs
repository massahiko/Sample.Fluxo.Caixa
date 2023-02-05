using Sample.Fluxo.Caixa.PlanoContas.Domain;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sample.Fluxo.Caixa.PlanoContas.Tests.Domain
{
    public class ContaFilterTests
    {
        [Fact]
        public void ContaFilter_Build_RetornaExpressaoDescricaoEquals()
        {
            // Arrange
            var saldoFilter = new ContaFilter()
            {
                Id = Guid.NewGuid(),
                Descricao = new List<string>() { "Teste" },
                Ativo = true,
                DataCadastro = DateTime.Now,
                ContaTipo = ContaTipo.SaldoInicial,
                TipoFiltroTexto = Core.Pageable.EnumTipoFiltroTexto.Equals,
            };

            // Act
            var result = saldoFilter.Build();

            // Assert
            var textFilter = new StringBuilder($"x => (((((True AndAlso (x.Id == value({typeof(ContaFilter).FullName}).Id)) AndAlso");
            textFilter.Append($" x.Descricao.ToLower().Equals(value({typeof(ContaFilter).FullName}+<>c__DisplayClass24_0).descricao.ToLower())) AndAlso")
                      .Append($" (Convert(x.Ativo, Nullable`1) == value({typeof(ContaFilter).FullName}).Ativo)) AndAlso")
                      .Append($" (Convert(x.DataCadastro, Nullable`1) == value({typeof(ContaFilter).FullName}).DataCadastro)) AndAlso")
                      .Append($" (Convert(x.ContaTipo, Int32) == Convert(value({typeof(ContaFilter).FullName}).ContaTipo, Int32)))");

            Assert.Equal(textFilter.ToString(), result.ToString());
        }

        [Fact]
        public void ContaFilter_Build_RetornaExpressaoDescricaoContains()
        {
            // Arrange
            var saldoFilter = new ContaFilter()
            {
                Id = Guid.NewGuid(),
                Descricao = new List<string>() { "Teste" },
                Ativo = true,
                DataCadastro = DateTime.Now,
                ContaTipo = ContaTipo.SaldoInicial,
                TipoFiltroTexto = Core.Pageable.EnumTipoFiltroTexto.Contains,
            };

            // Act
            var result = saldoFilter.Build();

            // Assert
            var textFilter = new StringBuilder($"x => (((((True AndAlso (x.Id == value({typeof(ContaFilter).FullName}).Id)) AndAlso");
            textFilter.Append($" x.Descricao.ToLower().Contains(value({typeof(ContaFilter).FullName}+<>c__DisplayClass24_0).descricao.ToLower())) AndAlso")
                      .Append($" (Convert(x.Ativo, Nullable`1) == value({typeof(ContaFilter).FullName}).Ativo)) AndAlso")
                      .Append($" (Convert(x.DataCadastro, Nullable`1) == value({typeof(ContaFilter).FullName}).DataCadastro)) AndAlso")
                      .Append($" (Convert(x.ContaTipo, Int32) == Convert(value({typeof(ContaFilter).FullName}).ContaTipo, Int32)))");

            Assert.Equal(textFilter.ToString(), result.ToString());
        }
    }
}
