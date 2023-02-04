using Sample.Fluxo.Caixa.Core.Messages;
using System;

namespace Sample.Fluxo.Caixa.Saldo.Application.Events
{
    public class AtualizarSaldoConsolidadoEvent : Event
    {
        public Guid LancamentoId { get; private set; }
        public DateTime DataEscrituracao { get; private set; }

        public AtualizarSaldoConsolidadoEvent(Guid lancamentoId, DateTime dataEscrituracao)
        {
            AggregateId = lancamentoId;
            LancamentoId = lancamentoId;
            DataEscrituracao = dataEscrituracao;
        }
    }
}
