using Sample.Fluxo.Caixa.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Domain;

namespace Sample.FluxoCaixa.PlanoContas.Domain
{
    public interface IContaRepository : IRepository<Conta>
    {
        Task<PagedResult<Conta>> ObterTodas(ContaFilter contaFilter);
        Task<Conta> ObterPorId(Guid id);
        void Adicionar(Conta conta);
        void Atualizar(Conta conta);
        void Excluir(Conta conta);
    }
}
