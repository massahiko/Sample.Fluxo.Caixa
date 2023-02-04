using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.Services
{
    public interface IContaAppService : IDisposable
    {
        Task<IEnumerable<ContaViewModel>> ObterPorTipo(ContaTipo contaTipo);
        Task<ContaViewModel> ObterPorId(Guid id);
        Task<IEnumerable<ContaViewModel>> ObterTodas();
        Task<ContaViewModel> AdicionarConta(ContaViewModel contaViewModel);
        Task<ContaViewModel> AtualizarConta(ContaViewModel contaViewModel);
        Task<bool> ExcluirConta(Guid contaId);
    }
}
