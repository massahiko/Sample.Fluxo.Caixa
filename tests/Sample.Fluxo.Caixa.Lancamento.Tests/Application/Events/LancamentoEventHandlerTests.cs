using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Application.Events;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.Lancamento.Tests.Application.Events
{
    public class LancamentoEventHandlerTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public LancamentoEventHandlerTests()
        {
            _mocker = new AutoMocker();
        }

        private LancamentoEventHandler CriarLancamentoEventHandler()
        {
            return _mocker.CreateInstance<LancamentoEventHandler>();
        }

        private Lancamento.Domain.Lancamento CriarLancamento()
        {
            return new Lancamento.Domain.Lancamento(Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
        }

        private CancelarLancamentoEvent CriarCancelarLancamentoEvent()
        {
            return new CancelarLancamentoEvent(Guid.NewGuid());
        }


        #region CancelarLancamentoEvent

        [Fact]
        public async Task LancamentoEventHandler_HandleCancelarLancamentoEvent_DeveCancelarComSucesso()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoEventHandler();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(CriarLancamento());

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await lancamentoEventHandler.Handle(CriarCancelarLancamentoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }


        [Fact]
        public async Task LancamentoEventHandler_HandleCancelarLancamentoEvent_DeveNotificarLancamentoNaoEncontrado()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoEventHandler();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<Lancamento.Domain.Lancamento>());

            // Act
            await lancamentoEventHandler.Handle(CriarCancelarLancamentoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<ILogger<LancamentoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoEventHandler_HandleCancelarLancamentoEvent_FalhaAoExcluirLancamento()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoEventHandler();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(CriarLancamento());

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            await lancamentoEventHandler.Handle(CriarCancelarLancamentoEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoEventHandler>>()
                .Verify(x => x.Log(LogLevel.Information,
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            _mocker.GetMock<ILogger<LancamentoEventHandler>>()
               .Verify(x => x.Log(LogLevel.Error,
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoEventHandler_HandleAtualizarSaldoInicialEvent_DeveRetornarException()
        {
            // Arrange
            var lancamentoEventHandler = CriarLancamentoEventHandler();

            _mocker.GetMock<ILancamentoRepository>()
                 .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await lancamentoEventHandler.Handle(CriarCancelarLancamentoEvent(), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion
    }
}
