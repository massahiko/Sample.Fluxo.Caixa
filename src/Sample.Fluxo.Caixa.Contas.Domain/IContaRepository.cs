using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Domain;
using System;
using System.Threading.Tasks;

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
