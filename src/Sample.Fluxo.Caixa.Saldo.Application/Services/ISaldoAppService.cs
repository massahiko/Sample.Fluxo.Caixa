using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Application.Services
{
    public interface ISaldoAppService
    {
        Task<IEnumerable<SaldoViewModel>> ObterTodos();
        Task<SaldoViewModel> ObterPorData(DateTime dateTime);
        Task<bool> ExcluirSaldo(Guid id);
        Task<byte[]> GerarRelatorio();        
    }
}
