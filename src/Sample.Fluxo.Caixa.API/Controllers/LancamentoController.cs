using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Application.Commands;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.API.Controllers
{
    /// <summary>
    /// Controller Lançamento
    /// </summary>
    [Route("[controller]")]
    public class LancamentoController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly ILancamentoQueries _lancamentoQueries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="mediatorHandler"></param>
        /// <param name="lancamentoQueries"></param>
        public LancamentoController(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediatorHandler,
            ILancamentoQueries lancamentoQueries) : base(notifications)
        {
            _mediatorHandler = mediatorHandler;
            _lancamentoQueries = lancamentoQueries;
        }

        /// <summary>
        /// Cria Lançamento.
        /// </summary>
        /// <remarks>
        /// Este método cadastra um novo lançamento.
        /// </remarks>
        /// <returns>True/False</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível criar o lançamento.</response>
        [ProducesResponseType(typeof(LancamentoViewModel), 200)]
        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(LancamentoViewModel lancamentoViewModel)
        {
            var command = new AdicionarLancamentoCommand(lancamentoViewModel.ContaId, lancamentoViewModel.Valor, lancamentoViewModel.DataEscrituracao);
            await _mediatorHandler.EnviarComando(command);
            lancamentoViewModel.Id = command.Id;

            return CustomResponse(lancamentoViewModel);
        }

        /// <summary>
        /// Edita lançamento.
        /// </summary>
        /// <remarks>
        /// Este método edita o lançamento com ID especificado.
        /// </remarks>
        /// <returns>True/False</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível editar o lançamento.</response>
        [ProducesResponseType(typeof(LancamentoViewModel), 200)]
        [HttpPut("Editar/{lancamentoId}")]
        public async Task<IActionResult> Editar(Guid lancamentoId, LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel != null)
                lancamentoViewModel.Id = lancamentoId;

            var command = new AtualizarLancamentoCommand(lancamentoViewModel.Id, lancamentoViewModel.ContaId, lancamentoViewModel.Valor, lancamentoViewModel.DataEscrituracao);
            await _mediatorHandler.EnviarComando(command);

            return CustomResponse(lancamentoViewModel);
        }

        /// <summary>
        /// Exclui lançamento.
        /// </summary>
        /// <remarks>
        /// Este método exclui o lançamento com ID especificado.
        /// </remarks>
        /// <returns>True/False</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível excluir o lançamento.</response>
        [HttpDelete("Excluir/{lancamentoId}")]
        public async Task<IActionResult> Excluir(Guid lancamentoId)
        {
            var command = new ExcluirLancamentoCommand(lancamentoId);

            return CustomResponse(await _mediatorHandler.EnviarComando(command));
        }

        /// <summary>
        /// Retorna a lista de lançamentos.
        /// </summary>
        /// <remarks>
        /// Este método retorna a lista de lançamentos.
        /// </remarks>
        /// <returns>Lista de Lançamentos</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter lista de lançamentos.</response>
        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos([FromQuery] LancamentoFilter lancamentoFilter)
        {
            return CustomResponse(await _lancamentoQueries.ObterTodos(lancamentoFilter));
        }

        /// <summary>
        /// Retorna o lançamento com id especificado.
        /// </summary>
        /// <remarks>
        /// Este método retorna o lançamento com id especificado.
        /// </remarks>
        /// <returns>Lançamento</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter o lançamento.</response>
        [HttpGet("ObterPorId/{lancamentoId}")]
        public async Task<IActionResult> ObterPorId(Guid lancamentoId)
        {
            return CustomResponse(await _lancamentoQueries.ObterPorId(lancamentoId));
        }
    }
}
