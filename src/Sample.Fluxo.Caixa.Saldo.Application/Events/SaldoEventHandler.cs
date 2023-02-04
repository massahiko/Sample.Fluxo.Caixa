using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Application.Events
{
    public class SaldoEventHandler :
        INotificationHandler<AtualizarSaldoInicialEvent>,
        INotificationHandler<AtualizarSaldoReceitaEvent>,
        INotificationHandler<AtualizarSaldoDespesaEvent>,
        INotificationHandler<AtualizarSaldoConsolidadoEvent>
    {
        private readonly ILogger<SaldoEventHandler> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly ISaldoRepository _saldoRepository;
        private readonly ISaldoConsolidadoService _saldoConsolidadoService;

        public SaldoEventHandler(
            ILogger<SaldoEventHandler> logger,
            IMediatorHandler mediatorHandler,
            ISaldoRepository lancamentoRepository,
            ISaldoConsolidadoService saldoConsolidadoService)
        {
            _logger = logger;
            _mediatorHandler = mediatorHandler;
            _saldoRepository = lancamentoRepository;
            _saldoConsolidadoService = saldoConsolidadoService;
        }

        public async Task Handle(AtualizarSaldoInicialEvent message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando evento {nameof(AtualizarSaldoInicialEvent)}");

                if (message.ExcluiuLancamento)
                {
                    if (!await ExcluirValorSaldo<AtualizarSaldoInicialEvent>(message.DataEscrituracao, message.Valor)) return;
                }
                else
                {
                    if (await ValidarExisteLancamentoSaldoInicial(message.DataEscrituracao, message.LancamentoId, true)) return;

                    var saldo = await _saldoRepository.ObterPorData(message.DataEscrituracao);
                    if (saldo == null)
                    {
                        saldo = new Domain.Saldo(message.DataEscrituracao, 0, 0);
                        saldo.AtualizarSaldoInicial(message.Valor);
                        _saldoRepository.Adicionar(saldo);
                    }
                    else
                    {
                        saldo.AtualizarSaldoInicial(message.Valor);
                        _saldoRepository.Atualizar(saldo);
                    }
                }

                var sucesso = await _saldoRepository.UnitOfWork.Commit();

                if (!sucesso)
                    await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                else
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoConsolidadoEvent(message.LancamentoId, message.DataEscrituracao));

                _logger.LogInformation($"Finalizando evento {nameof(AtualizarSaldoInicialEvent)}");
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar evento {nameof(AtualizarSaldoInicialEvent)}");
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                throw;
            }
        }

        public async Task Handle(AtualizarSaldoReceitaEvent message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando evento {nameof(AtualizarSaldoReceitaEvent)}");

                if (message.ExcluiuLancamento)
                {
                    if (!await ExcluirValorSaldo<AtualizarSaldoReceitaEvent>(message.DataEscrituracao, message.Valor)) return;
                }
                else
                {
                    if (await ValidarExisteLancamentoSaldoInicial(message.DataEscrituracao, message.LancamentoId)) return;

                    var saldo = await _saldoRepository.ObterPorData(message.DataEscrituracao);
                    if (saldo == null)
                    {
                        saldo = new Domain.Saldo(message.DataEscrituracao, message.Valor, 0);
                        _saldoRepository.Adicionar(saldo);
                    }
                    else
                    {
                        saldo.AtualizarReceita(message.Valor);
                        _saldoRepository.Atualizar(saldo);
                    }
                }

                var sucesso = await _saldoRepository.UnitOfWork.Commit();

                if (!sucesso)
                    await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                else
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoConsolidadoEvent(message.LancamentoId, message.DataEscrituracao));

                _logger.LogInformation($"Finalizando evento {nameof(AtualizarSaldoReceitaEvent)}");
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar evento {nameof(AtualizarSaldoReceitaEvent)}");
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                throw;
            }
        }

        public async Task Handle(AtualizarSaldoDespesaEvent message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando evento {nameof(AtualizarSaldoDespesaEvent)}");

                if (message.ExcluiuLancamento)
                {
                    if (!await ExcluirValorSaldo<AtualizarSaldoDespesaEvent>(message.DataEscrituracao, message.Valor)) return;
                }
                else
                {
                    if (await ValidarExisteLancamentoSaldoInicial(message.DataEscrituracao, message.LancamentoId)) return;

                    var saldo = await _saldoRepository.ObterPorData(message.DataEscrituracao);
                    if (saldo == null)
                    {
                        saldo = new Domain.Saldo(message.DataEscrituracao, 0, message.Valor);
                        _saldoRepository.Adicionar(saldo);
                    }
                    else
                    {
                        saldo.AtualizarDespesa(message.Valor);
                        _saldoRepository.Atualizar(saldo);
                    }
                }

                var sucesso = await _saldoRepository.UnitOfWork.Commit();

                if (!sucesso)
                    await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                else
                    await _mediatorHandler.PublicarEvento(new AtualizarSaldoConsolidadoEvent(message.LancamentoId, message.DataEscrituracao));

                _logger.LogInformation($"Finalizando evento {nameof(AtualizarSaldoDespesaEvent)}");
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar evento {nameof(AtualizarSaldoDespesaEvent)}");
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                throw;
            }
        }

        public async Task Handle(AtualizarSaldoConsolidadoEvent message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando evento {nameof(AtualizarSaldoConsolidadoEvent)}");

                var sucesso = await _saldoConsolidadoService.AtualizarSaldos(message.DataEscrituracao);
                if (!sucesso)
                    await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));

                _logger.LogInformation($"Finalizando evento {nameof(AtualizarSaldoConsolidadoEvent)}");
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar evento {nameof(AtualizarSaldoConsolidadoEvent)}");
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(message.LancamentoId));
                throw;
            }
        }

        private async Task<bool> ExcluirValorSaldo<IEvent>(
            DateTime dataEscrituracao, 
            decimal valor) where IEvent : IntegrationEvent
        {
            var saldo = await _saldoRepository.ObterPorData(dataEscrituracao);
            if (saldo == null)
            {
                _logger.LogInformation($"Saldo não encontrado!");
                return false;
            }

            switch (typeof(IEvent).Name)
            {
                case nameof(AtualizarSaldoInicialEvent):
                    saldo.AtualizarSaldoInicial(valor);
                    break;
                case nameof(AtualizarSaldoReceitaEvent):
                    saldo.AtualizarReceita(valor);
                    break;
                case nameof(AtualizarSaldoDespesaEvent):
                    saldo.AtualizarDespesa(valor);
                    break;
                default:
                    break;
            }
            
            _saldoRepository.Atualizar(saldo);
            return true;
        }

        private async Task<bool> ValidarExisteLancamentoSaldoInicial(
            DateTime dataEscrituracao,
            Guid lancamentoId,
            bool validarSomenteSaldoInicialOutraData = false)
        {
            if (!await _saldoRepository.ValidarExisteSaldoInicialOutraData(dataEscrituracao)) return false;

            if (validarSomenteSaldoInicialOutraData)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("Saldo", "Existe lançamento de saldo inicial superior a data informada"));
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(lancamentoId));
            }

            var saldoInicialSuperior = !await _saldoRepository.ValidarExisteSaldoInicialInferiorOuIgual(dataEscrituracao);
            if (saldoInicialSuperior)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("Saldo", "Existe lançamento de saldo inicial superior a data informada"));
                await _mediatorHandler.PublicarEvento(new CancelarLancamentoEvent(lancamentoId));
                return true;
            }

            return false;
        }
    }
}
