using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Queries
{
    public interface ILancamentoQueries
    {
        Task<IEnumerable<LancamentoViewModel>> ObterTodos();
        Task<LancamentoViewModel> ObterPorId(Guid lancamentoId);
    }
}
