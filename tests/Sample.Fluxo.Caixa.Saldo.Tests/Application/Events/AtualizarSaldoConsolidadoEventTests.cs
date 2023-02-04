using Sample.Fluxo.Caixa.Saldo.Application.Events;
using System;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Application.Events
{
    public class AtualizarSaldoConsolidadoEventTests
    {
        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", "2023-01-01")]
        public void AtualizarSaldoConsolidadolEvent_CriarEvento_DeveCriarComSucesso(Guid lancamentoId, DateTime dateTime)
        {
            // Arrange & Act
            var atualizarSaldoConsolidadolEvent = new AtualizarSaldoConsolidadoEvent(lancamentoId, dateTime);

            // Assert
            Assert.NotNull(atualizarSaldoConsolidadolEvent);
            Assert.Equal(atualizarSaldoConsolidadolEvent.AggregateId, lancamentoId);
            Assert.Equal(atualizarSaldoConsolidadolEvent.LancamentoId, lancamentoId);
            Assert.Equal(atualizarSaldoConsolidadolEvent.DataEscrituracao, dateTime);
        }
    }
}
