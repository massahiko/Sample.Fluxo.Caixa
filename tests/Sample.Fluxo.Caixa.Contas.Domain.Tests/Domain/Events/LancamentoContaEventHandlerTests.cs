using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.FluxoCaixa.PlanoContas.Domain.Events;
using System;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Sample.FluxoCaixa.PlanoContas.Domain;
using Bogus;
using System.Linq;

namespace Sample.Fluxo.Caixa.PlanoContas.Tests.Domain.Events
{
    public class LancamentoContaEventHandlerTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public LancamentoContaEventHandlerTests()
        {
            _mocker = new AutoMocker();
        }

        private LancamentoContaEventHandler CriarLancamentoContaEventHandler()
        {
            return _mocker.CreateInstance<LancamentoContaEventHandler>();
        }

        private Conta CriarConta(ContaTipo contaTipo)
        {
            return new Faker<Conta>("pt_BR")
                .CustomInstantiator(f => new Conta(f.Name.FullName(),
                                                    true,
                                                    contaTipo,
                                                    f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5)))
                ).Generate(1)
                .FirstOrDefault();
        }

        #region LancamentoAdicionadoEvent

        private LancamentoAdicionadoEvent CriarLancamentoAdicionadoEvent()
        {
            return new LancamentoAdicionadoEvent(Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
        }

        private LancamentoAtualizadoEvent CriarLancamentoAtualizadoEvent()
        {
            return new LancamentoAtualizadoEvent(Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
        }

        private LancamentoExcluidoEvent CriarLancamentoExcluidoEvent()
        {
            return new LancamentoExcluidoEvent(Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
        }

        [Fact]
        public async Task LancamentoContaEventHandler_HandleLancamentoAdicionadoEvent_DevePublicarEventoAtualizarSaldoInicial()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoContaEventHandler();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(CriarConta(ContaTipo.SaldoInicial));

            // Act
            await lancamentoEventHandler.Handle(CriarLancamentoAdicionadoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoInicialEvent>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoReceitaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoDespesaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoContaEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task LancamentoContaEventHandler_HandleLancamentoAtualizadoEvent_DevePublicarEventoAtualizarReceita()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoContaEventHandler();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(CriarConta(ContaTipo.Receita));

            // Act
            await lancamentoEventHandler.Handle(CriarLancamentoAtualizadoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoInicialEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoReceitaEvent>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoDespesaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoContaEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task LancamentoContaEventHandler_HandleLancamentoExcluidoEvent_DevePublicarEventoAtualizarDespesa()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoContaEventHandler();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(CriarConta(ContaTipo.Despesa));

            // Act
            await lancamentoEventHandler.Handle(CriarLancamentoExcluidoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoInicialEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoReceitaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoDespesaEvent>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoContaEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task LancamentoContaEventHandler_HandleLancamentoAdicionadoEvent_NaoEncontrouContaDeveNotificarCancelarLancamento()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoContaEventHandler();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(It.IsAny<Conta>());

            // Act
            await lancamentoEventHandler.Handle(CriarLancamentoAdicionadoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoInicialEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoReceitaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoDespesaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<ILogger<LancamentoContaEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task LancamentoContaEventHandler_HandleLancamentoAdicionadoEvent_DeveRetornarException()
        {
            // Arrange
            Exception exception = null;
            var lancamentoEventHandler = CriarLancamentoContaEventHandler();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .Throws(new Exception(_message));

            // Act
            try
            {
                await lancamentoEventHandler.Handle(CriarLancamentoAdicionadoEvent(), It.IsAny<CancellationToken>());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoInicialEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoReceitaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<AtualizarSaldoDespesaEvent>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoContaEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(3));
        }

        #endregion
    }
}
