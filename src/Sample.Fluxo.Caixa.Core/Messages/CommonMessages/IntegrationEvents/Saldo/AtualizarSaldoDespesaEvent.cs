using System;

namespace Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo
{
    public class AtualizarSaldoDespesaEvent : IntegrationEvent
    {
        public Guid LancamentoId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataEscrituracao { get; private set; }
        public bool ExcluiuLancamento { get; private set; }

        public AtualizarSaldoDespesaEvent(Guid lancamentoId, decimal valor, DateTime dataEscrituracao)
        {
            AggregateId = lancamentoId;
            LancamentoId = lancamentoId;
            Valor = valor;
            DataEscrituracao = dataEscrituracao;
            ExcluiuLancamento = valor < 0;
        }
    }
}
