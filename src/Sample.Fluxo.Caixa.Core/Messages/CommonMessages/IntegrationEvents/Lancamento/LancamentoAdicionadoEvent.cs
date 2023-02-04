using System;

namespace Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento
{
    public class LancamentoAdicionadoEvent : IntegrationEvent
    {
        public Guid LancamentoId { get; protected set; }
        public Guid ContaId { get; protected set; }
        public decimal Valor { get; protected set; }
        public DateTime DataEscrituracao { get; protected set; }

        //public LancamentoAdicionadoEvent() { }

        public LancamentoAdicionadoEvent(Guid lancamentoId, Guid contaId, decimal valor, DateTime dataEscrituracao)
        {
            AggregateId = lancamentoId;
            LancamentoId = lancamentoId;
            ContaId = contaId;
            Valor = valor;
            DataEscrituracao = dataEscrituracao;
        }
    }
}
