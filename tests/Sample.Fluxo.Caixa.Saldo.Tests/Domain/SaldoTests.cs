using System;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Domain
{
    public class SaldoTests
    {

        [Theory]
        [InlineData("2023-01-01", 10, 20)]
        public void Saldo_CriarEntidade_DeveCriarComSucesso(
            DateTime dateEscrituracao,
            decimal receita,
            decimal despesa)
        {
            // Arrange & Act
            var saldo = new Saldo.Domain.Saldo(dateEscrituracao, receita, despesa);

            // Assert
            Assert.NotNull(saldo);
            Assert.Equal(receita - despesa, saldo.SaldoFinal);
            Assert.Equal(dateEscrituracao, saldo.DataEscrituracao);
        }

        [Theory]
        [InlineData("2023-01-01", 10, 20, 100)]
        public void Saldo_AtualizarSaldoInicial_DeveAtualizarComSucessoAcumulando(
            DateTime dateEscrituracao,
            decimal receita,
            decimal despesa,
            decimal saldoInicial)
        {
            // Arrange & Act
            var saldo = new Saldo.Domain.Saldo(dateEscrituracao, receita, despesa);
            saldo.AtualizarSaldoInicial(saldoInicial);
            saldo.AtualizarSaldoInicial(saldoInicial);

            // Assert
            Assert.NotNull(saldo);
            Assert.Equal((saldoInicial * 2) + receita - despesa, saldo.SaldoFinal);
        }

        [Theory]
        [InlineData("2023-01-01", 10, 20, 100, 200)]
        public void Saldo_AtualizarSaldoInicial_DeveAtualizarComSucessoSemAcumular(
            DateTime dateEscrituracao,
            decimal receita,
            decimal despesa,
            decimal saldoInicial,
            decimal saldoInicialNovo)
        {
            // Arrange & Act
            var saldo = new Saldo.Domain.Saldo(dateEscrituracao, receita, despesa);
            saldo.AtualizarSaldoInicial(saldoInicial);
            saldo.AtualizarSaldoInicial(saldoInicialNovo, false);

            // Assert
            Assert.NotNull(saldo);
            Assert.Equal(saldoInicialNovo + receita - despesa, saldo.SaldoFinal);
        }

        [Theory]
        [InlineData("2023-01-01", 10, 20, 100)]
        public void Saldo_AtualizarReceita_DeveAtualizarComSucesso(
            DateTime dateEscrituracao,
            decimal receita,
            decimal despesa,
            decimal receitaAtualizada)
        {
            // Arrange & Act
            var saldo = new Saldo.Domain.Saldo(dateEscrituracao, receita, despesa);
            saldo.AtualizarReceita(receitaAtualizada);

            // Assert
            Assert.NotNull(saldo);
            Assert.Equal((receita + receitaAtualizada) - despesa, saldo.SaldoFinal);
        }

        [Theory]
        [InlineData("2023-01-01", 10, 20, 100)]
        public void Saldo_AtualizarDespesa_DeveAtualizarComSucesso(
            DateTime dateEscrituracao,
            decimal receita,
            decimal despesa,
            decimal despesaAtualizada)
        {
            // Arrange & Act
            var saldo = new Saldo.Domain.Saldo(dateEscrituracao, receita, despesa);
            saldo.AtualizarDespesa(despesaAtualizada);

            // Assert
            Assert.NotNull(saldo);
            Assert.Equal(receita - (despesa + despesaAtualizada), saldo.SaldoFinal);
        }
    }
}
