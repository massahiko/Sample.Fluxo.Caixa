using Sample.Fluxo.Caixa.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Sample.FluxoCaixa.PlanoContas.Domain
{
    public interface IContaRepository : IRepository<Conta>
    {
        Task<IEnumerable<Conta>> ObterTodas();
        Task<Conta> ObterPorId(Guid id);
        Task<IEnumerable<Conta>> ObterPorTipo(ContaTipo contaTipo);
        void Adicionar(Conta conta);
        void Atualizar(Conta conta);
        void Excluir(Conta conta);
    }
}
