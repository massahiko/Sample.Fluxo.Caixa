using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Domain
{
    public class SaldoConsolidadoService : ISaldoConsolidadoService
    {
        private readonly ISaldoRepository _saldoRepository;
        private readonly IMediatorHandler _mediatorHandler;

        public SaldoConsolidadoService(
            ISaldoRepository saldoRepository,
            IMediatorHandler mediatorHandler)
        {
            _saldoRepository = saldoRepository;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> AtualizarSaldos(DateTime dateTime)
        {
            var saldos = (await _saldoRepository.ObterTodos(new SaldoFilter())).Data.OrderBy(d => d.DataEscrituracao);
            var saldoFinal = saldos.FirstOrDefault()?.SaldoFinal;

            if (saldos.Count() <= 1) return true;

            foreach (var item in saldos.Skip(1))
            {
                item.AtualizarSaldoInicial(saldoFinal.GetValueOrDefault(), false);
                saldoFinal = item.SaldoFinal;
                _saldoRepository.Atualizar(item);
            }

            var sucesso = await _saldoRepository.UnitOfWork.Commit();
            if (!sucesso)
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("Saldo", "Não foi possível atualizar os Saldos"));

            return sucesso;
        }
    }
}
