using System;

namespace Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento
{
    public class CancelarLancamentoEvent : IntegrationEvent
    {
        public Guid LancamentoId { get; protected set; }

        public CancelarLancamentoEvent(Guid lancamentoId)
        {
            AggregateId = lancamentoId;
            LancamentoId = lancamentoId;
        }
    }
}
