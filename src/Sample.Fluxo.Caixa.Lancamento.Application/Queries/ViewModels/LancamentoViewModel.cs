using Sample.Fluxo.Caixa.Core.Data.EventSourcing;
using System;
using System.Collections.Generic;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels
{
    public class LancamentoViewModel
    {
        public Guid Id { get; set; }

        public decimal Valor { get; set; }

        public DateTime DataEscrituracao { get; set; }

        public Guid ContaId { get; set; }

        public IEnumerable<StoredEvent> HistoricoEventos { get; set; }
    }
}
