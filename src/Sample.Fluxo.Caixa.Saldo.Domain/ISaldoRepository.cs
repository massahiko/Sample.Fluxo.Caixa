using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Domain
{
    public interface ISaldoRepository : IRepository<Saldo>
    {
        Task<PagedResult<Saldo>> ObterTodos(SaldoFilter saldoFilter);
        Task<IEnumerable<Saldo>> ObterListaMaiorIgualData(DateTime dateTime);
        Task<Saldo> ObterPorData(DateTime dateTime);
        Task<bool> ValidarExisteSaldoInicialOutraData(DateTime dateTime);
        Task<bool> ValidarExisteSaldoInicialInferiorOuIgual(DateTime dateTime);

        void Adicionar(Saldo saldo);
        void Atualizar(Saldo saldo);
        void Excluir(Guid id);
    }
}
