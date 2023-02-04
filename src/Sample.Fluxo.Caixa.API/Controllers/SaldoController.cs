using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Saldo.Application.Services;
using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.API.Controllers
{
    /// <summary>
    /// Controller Lançamento
    /// </summary>
    [Route("[controller]")]
    public class SaldoController : MainController
    {
        private readonly ISaldoAppService _saldoAppService;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="saldoAppService"></param>
        public SaldoController(
            INotificationHandler<DomainNotification> notifications,
            ISaldoAppService saldoAppService) : base(notifications)
        {
            _saldoAppService = saldoAppService;
        }

        /// <summary>
        /// Retorna a lista de saldos.
        /// </summary>
        /// <remarks>
        /// Este método retorna a lista de saldos.
        /// </remarks>
        /// <returns>Lista de Saldos</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter lista de saldos.</response>
        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            return CustomResponse(await _saldoAppService.ObterTodos());
        }

        /// <summary>
        /// Retorna o saldo com a data especificada.
        /// </summary>
        /// <remarks>
        /// Este método retorna o saldo com data especificado.
        /// </remarks>
        /// <returns>Saldo</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter o Saldo.</response>
        [HttpGet("ObterPorData/{data}")]
        public async Task<IActionResult> ObterPorData(DateTime data)
        {
            return CustomResponse(await _saldoAppService.ObterPorData(data));
        }

        /// <summary>
        /// Exclui o saldo com o id especificado.
        /// </summary>
        /// <remarks>
        /// Este método Exclui o saldo com o id especificado.
        /// </remarks>
        /// <returns>True/False</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter o Saldo.</response>
        [HttpDelete("Excluir/{saldoId}")]
        public async Task<IActionResult> Excluir(Guid saldoId)
        {
            return CustomResponse(await _saldoAppService.ExcluirSaldo(saldoId));
        }

        /// <summary>
        /// Retorna o relatório de saldo consolidado 
        /// </summary>
        /// <remarks>
        /// Este método retorna o relatório de saldo consolidado 
        /// </remarks>
        /// <returns>Arquivo relatório</returns>
        /// <response code="200">Sucesso na requisição</response>
        /// <response code="400">Os parâmetros não foram passados corretamente ou ocorreu algum erro inesperado durante a execução do método</response>
        /// <response code="500">Ops! Não foi possível obter o Saldo.</response>
        [HttpGet("GerarRelatorio")]
        public async Task<IActionResult> GerarRelatorio()
        {
            var relatorio = await _saldoAppService.GerarRelatorio();

            return File(relatorio, "text/csv", $"Relatorio_Fluxo_Caixa_Consolidado_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.csv");
        }
    }
}
