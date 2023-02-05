using System.Threading.Tasks;
using System;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Lancamento.Domain;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Queries
{
    public interface ILancamentoQueries
    {
        Task<PagedResult<LancamentoViewModel>> ObterTodos(LancamentoFilter lancamentoFilter);
        Task<LancamentoViewModel> ObterPorId(Guid lancamentoId);
    }
}
