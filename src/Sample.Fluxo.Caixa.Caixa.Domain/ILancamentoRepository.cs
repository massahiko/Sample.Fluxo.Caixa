using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Lancamento.Domain
{
    public interface ILancamentoRepository : IRepository<Lancamento>
    {
        Task<PagedResult<Lancamento>> ObterTodos(LancamentoFilter lancamentoFilter, bool asNoTracking = false);
        Task<Lancamento> ObterPorId(Guid id, bool asNoTracking = false);

        void Adicionar(Lancamento lancamento);
        void Atualizar(Lancamento lancamento);
        void Excluir(Lancamento lancamento);
    }
}
