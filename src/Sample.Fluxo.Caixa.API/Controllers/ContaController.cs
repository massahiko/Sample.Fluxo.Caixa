using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.PlanoContas.Application.Services;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.API.Controllers
{
    /// <summary>
    /// Controller Conta
    /// </summary>
    [Route("[controller]")]
    public class ContaController : MainController
    {
        private readonly IContaAppService _contaAppService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="contaAppService"></param>
        public ContaController(INotificationHandler<DomainNotification> notifications,
                               IContaAppService contaAppService) : base(notifications)
        {
            _contaAppService = contaAppService;
        }

        /// <summary>
        /// Cria Conta.
        /// </summary>
        /// <remarks>
        /// Este método cadastra uma nova conta.
        /// </remarks>
        /// <returns>ContaViewModel</returns>
        /// <response code="200">Sucesso na requisição type="ContaViewModel"</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível criar a Conta.</response>
        [ProducesResponseType(typeof(ContaViewModel), 200)]
        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(ContaViewModel contaViewModel)
        {
            return CustomResponse(await _contaAppService.AdicionarConta(contaViewModel));
        }

        /// <summary>
        /// Edita Conta.
        /// </summary>
        /// <remarks>
        /// Este método edita a conta com ID especificado.
        /// </remarks>
        /// <returns>ContaViewModel</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível editar a Conta.</response>
        [ProducesResponseType(typeof(ContaViewModel), 200)]
        [HttpPut("Editar/{contaId}")]
        public async Task<IActionResult> Editar(Guid contaId, ContaViewModel contaViewModel)
        {
            if (contaViewModel != null)
                contaViewModel.Id = contaId;

            return CustomResponse(await _contaAppService.AtualizarConta(contaViewModel));
        }

        /// <summary>
        /// Exclui Conta.
        /// </summary>
        /// <remarks>
        /// Este método exclui a conta com ID especificado.
        /// </remarks>
        /// <returns>True/False</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível editar a Conta.</response>
        [ProducesResponseType(typeof(bool), 200)]
        [HttpDelete("Excluir/{contaId}")]
        public async Task<IActionResult> Excluir(Guid contaId)
        {
            return CustomResponse(await _contaAppService.ExcluirConta(contaId));
        }

        /// <summary>
        /// Retorna a lista de contas.
        /// </summary>
        /// <remarks>
        /// Este método retorna a lista de contas.
        /// </remarks>
        /// <returns>Lista de Contas</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter lista de contas.</response>
        [HttpGet("ObterTodas")]
        public async Task<IActionResult> ObterTodas()
        {
            return CustomResponse(await _contaAppService.ObterTodas());
        }

        /// <summary>
        /// Retorna a conta com id especificado.
        /// </summary>
        /// <remarks>
        /// Este método retorna a conta com id especificado.
        /// </remarks>
        /// <returns>Conta</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter a Conta.</response>
        [HttpGet("ObterPorId/{contaId}")]
        public async Task<IActionResult> ObterPorId(Guid contaId)
        {
            return CustomResponse(await _contaAppService.ObterPorId(contaId));
        }

        /// <summary>
        /// Retorna a conta por tipo especificado.
        /// </summary>
        /// <remarks>
        /// Este método retorna a conta com id especificado.
        /// </remarks>
        /// <returns>Lista de Contas</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter a Conta.</response>
        [HttpGet("ObterPorTipo/{contaTipo}")]
        public async Task<IActionResult> ObterPorTipo(ContaTipo contaTipo)
        {
            return CustomResponse(await _contaAppService.ObterPorTipo(contaTipo));
        }
    }
}
