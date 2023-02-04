using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Events
{
    public class LancamentoEventHandler :
        INotificationHandler<CancelarLancamentoEvent>
    {
        private readonly ILogger<LancamentoEventHandler> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly ILancamentoRepository _lancamentoRepository;

        public LancamentoEventHandler(
            ILogger<LancamentoEventHandler> logger,
            IMediatorHandler mediatorHandler,
            ILancamentoRepository lancamentoRepository)
        {
            _logger = logger;
            _mediatorHandler = mediatorHandler;
            _lancamentoRepository = lancamentoRepository;
        }

        public async Task Handle(CancelarLancamentoEvent message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Executando evento {nameof(CancelarLancamentoEvent)}");

                var lancamento = await _lancamentoRepository.ObterPorId(message.LancamentoId);
                if (lancamento == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Lançamento", "Lançamento não encontrado!"));
                    return;
                }

                _lancamentoRepository.Excluir(lancamento);

                var sucesso = await _lancamentoRepository.UnitOfWork.Commit();
                if (!sucesso)
                    _logger.LogError($"Falha ao Salvar lançamentos evento {message.LancamentoId}");
                else
                    _logger.LogInformation($"Finalizando evento {nameof(CancelarLancamentoEvent)}");

            }
            catch (System.Exception)
            {
                _logger.LogError($"Falha ao executar evento {nameof(CancelarLancamentoEvent)}");
                throw;
            }
        }
    }
}
