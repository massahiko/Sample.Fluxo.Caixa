using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Commands
{
    public class LancamentoCommandHandler :
        IRequestHandler<AdicionarLancamentoCommand, bool>,
        IRequestHandler<AtualizarLancamentoCommand, bool>,
        IRequestHandler<ExcluirLancamentoCommand, bool>
    {
        private readonly ILogger<LancamentoCommandHandler> _logger;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IMediatorHandler _mediatorHandler;

        public LancamentoCommandHandler(
            ILogger<LancamentoCommandHandler> logger,
            ILancamentoRepository lancamentoRepository,
            IMediatorHandler mediatorHandler)
        {
            _logger = logger;
            _lancamentoRepository = lancamentoRepository;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> Handle(AdicionarLancamentoCommand message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando command {nameof(AdicionarLancamentoCommand)}");

                if (!ValidarComando(message)) return false;

                var lancamento = new Domain.Lancamento(message.Id, message.ContaId, message.Valor, message.DataEscrituracao);
                _lancamentoRepository.Adicionar(lancamento);

                lancamento.AdicionarEvento(new LancamentoAdicionadoEvent(lancamento.Id, lancamento.ContaId, lancamento.Valor, lancamento.DataEscrituracao));

                return await _lancamentoRepository.UnitOfWork.Commit();
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar command {nameof(AdicionarLancamentoCommand)}");
                throw;
            }
        }

        public async Task<bool> Handle(AtualizarLancamentoCommand message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando command {nameof(AtualizarLancamentoCommand)}");

                if (!ValidarComando(message)) return false;

                var lancamento = await _lancamentoRepository.ObterPorId(message.LancamentoId, true);
                if (lancamento == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Lançamento", "Lançamento não encontrado!"));
                    return false;
                }

                var diferencaSaldo = message.Valor - lancamento.Valor;

                lancamento = new Domain.Lancamento(message.LancamentoId, message.ContaId, message.Valor, message.DataEscrituracao);
                _lancamentoRepository.Atualizar(lancamento);

                lancamento.AdicionarEvento(new LancamentoAtualizadoEvent(lancamento.Id, lancamento.ContaId, diferencaSaldo, lancamento.DataEscrituracao));

                return await _lancamentoRepository.UnitOfWork.Commit();
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar command {nameof(AtualizarLancamentoCommand)}");
                throw;
            }
        }

        public async Task<bool> Handle(ExcluirLancamentoCommand message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando command {nameof(ExcluirLancamentoCommand)}");

                if (!ValidarComando(message)) return false;

                var lancamento = await _lancamentoRepository.ObterPorId(message.LancamentoId);
                if (lancamento == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Lançamento", "Lançamento não encontrado!"));
                    return false;
                }

                _lancamentoRepository.Excluir(lancamento);

                var sucesso = await _lancamentoRepository.UnitOfWork.Commit();

                if (sucesso)
                    await _mediatorHandler.PublicarEvento(new LancamentoExcluidoEvent(lancamento.Id, lancamento.ContaId, lancamento.Valor, lancamento.DataEscrituracao));

                return sucesso;
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar command {nameof(ExcluirLancamentoCommand)}");
                throw;
            }
        }

        private bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                _mediatorHandler.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));
            }

            return false;
        }
    }
}
