using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.FluxoCaixa.PlanoContas.Domain.Events
{
    public class LancamentoContaEventHandler :
        INotificationHandler<LancamentoAdicionadoEvent>,
        INotificationHandler<LancamentoAtualizadoEvent>,
        INotificationHandler<LancamentoExcluidoEvent>
    {
        private readonly ILogger<LancamentoContaEventHandler> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IContaRepository _contaRepository;

        public LancamentoContaEventHandler(
            ILogger<LancamentoContaEventHandler> logger,
            IMediatorHandler mediatorHandler,
            IContaRepository contaRepository)
        {
            _logger = logger;
            _mediatorHandler = mediatorHandler;
            _contaRepository = contaRepository;
        }

        public async Task Handle(LancamentoAdicionadoEvent message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executando evento {nameof(LancamentoAdicionadoEvent)}");

            var contaTipo = await ObterTipoConta(message.ContaId, message.LancamentoId);
            await PublicarEventoAtualizarSaldo(contaTipo, message.LancamentoId, message.Valor, message.DataEscrituracao);
        }

        public async Task Handle(LancamentoAtualizadoEvent message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executando evento {nameof(LancamentoAtualizadoEvent)}");

            var contaTipo = await ObterTipoConta(message.ContaId, message.LancamentoId);
            await PublicarEventoAtualizarSaldo(contaTipo, message.LancamentoId, message.Valor, message.DataEscrituracao);
        }

        public async Task Handle(LancamentoExcluidoEvent message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executando evento {nameof(LancamentoExcluidoEvent)}");

            var contaTipo = await ObterTipoConta(message.ContaId, message.LancamentoId);
            await PublicarEventoAtualizarSaldo(contaTipo, message.LancamentoId, message.Valor, message.DataEscrituracao);
        }

        private async Task<ContaTipo> ObterTipoConta(Guid contaId, Guid lancamentoId)
        {
            try
            {
                _logger.LogInformation($"Obter Tipo de Conta");

                var conta = await _contaRepository.ObterPorId(contaId);

                if (conta == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Conta", "Id da Conta informado no lançamento não encontrado!"));
                    await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(lancamentoId));

                    return default;
                }

                return conta.ContaTipo;
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao Obter Tipo de Conta");
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(lancamentoId));
                throw;
            }
        }

        private async Task PublicarEventoAtualizarSaldo(ContaTipo contaTipo, Guid lancamentoId, decimal valor, DateTime dataEscrituracao)
        {
            switch (contaTipo)
            {
                case ContaTipo.SaldoInicial:
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoInicialEvent(lancamentoId, valor, dataEscrituracao));
                    break;
                case ContaTipo.Receita:
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoReceitaEvent(lancamentoId, valor, dataEscrituracao));
                    break;
                case ContaTipo.Despesa:
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoDespesaEvent(lancamentoId, valor, dataEscrituracao));
                    break;
                default:
                    break;
            }
        }
    }
}
