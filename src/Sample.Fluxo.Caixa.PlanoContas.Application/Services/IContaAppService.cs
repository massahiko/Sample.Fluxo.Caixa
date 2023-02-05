using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.Fluxo.Caixa.PlanoContas.Domain;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.Services
{
    public interface IContaAppService : IDisposable
    {
        Task<PagedResult<ContaViewModel>> ObterPorTipo(ContaTipo contaTipo);
        Task<ContaViewModel> ObterPorId(Guid id);
        Task<PagedResult<ContaViewModel>> ObterTodas(ContaFilter contaFilter);
        Task<ContaViewModel> AdicionarConta(ContaViewModel contaViewModel);
        Task<ContaViewModel> AtualizarConta(ContaViewModel contaViewModel);
        Task<bool> ExcluirConta(Guid contaId);
    }
}
