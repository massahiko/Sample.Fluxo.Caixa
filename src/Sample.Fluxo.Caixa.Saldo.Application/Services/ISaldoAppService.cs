using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Application.Services
{
    public interface ISaldoAppService
    {
        Task<PagedResult<SaldoViewModel>> ObterTodos(SaldoFilter saldoFilter);
        Task<SaldoViewModel> ObterPorData(DateTime dateTime);
        Task<bool> ExcluirSaldo(Guid id);
        Task<byte[]> GerarRelatorio();        
    }
}
