using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Saldo.Application.Events;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Application.Events
{
    public class SaldoEventHandlerTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public SaldoEventHandlerTests()
        {
            _mocker = new AutoMocker();
        }

        private SaldoEventHandler CriarSaldoEventHandler()
        {
            return _mocker.CreateInstance<SaldoEventHandler>();
        }

        #region AtualizarSaldoInicialEvent

        private AtualizarSaldoInicialEvent CriarAtualizarSaldoInicialEvent()
        {
            return new AtualizarSaldoInicialEvent(Guid.NewGuid(), 100, DateTime.Now);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoInicialEvent_DeveAdicionarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(It.IsAny<Saldo.Domain.Saldo>());

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoInicialEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoInicialEvent_DeveAtualizarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoInicialEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoInicialEvent_DeveNotificarSaldoInicialComDataAnteriorExistente()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(true);
                        
            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoInicialEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Exactly(2));
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Exactly(2));
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoInicialEvent_FalhaAoSalvarSaldo()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoInicialEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoInicialEvent_DeveRetornarException()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoEventHandler.Handle(CriarAtualizarSaldoInicialEvent(), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AtualizarSaldoReceitaEvent

        private AtualizarSaldoReceitaEvent CriarAtualizarSaldoReceitaEvent()
        {
            return new AtualizarSaldoReceitaEvent(Guid.NewGuid(), 100, DateTime.Now);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoReceitaEvent_DeveAdicionarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(It.IsAny<Saldo.Domain.Saldo>());

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoReceitaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoReceitaEvent_DeveAtualizarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoReceitaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoReceitaEvent_DeveNotificarSaldoInicialComDataSuperiorExistente()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoReceitaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoReceitaEvent_FalhaAoSalvarSaldo()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoReceitaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoReceitaEvent_DeveRetornarException()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoEventHandler.Handle(CriarAtualizarSaldoReceitaEvent(), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AtualizarSaldoReceitaEvent

        private AtualizarSaldoDespesaEvent CriarAtualizarSaldoDespesaEvent()
        {
            return new AtualizarSaldoDespesaEvent(Guid.NewGuid(), 100, DateTime.Now);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoDespesaEvent_DeveAdicionarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(It.IsAny<Saldo.Domain.Saldo>());

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoDespesaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoDespesaEvent_DeveAtualizarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoDespesaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoDespesaEvent_DeveNotificarSaldoInicialComDataSuperiorExistente()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()))
               .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoDespesaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoDespesaEvent_FalhaAoSalvarSaldo()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .ReturnsAsync(false);

            Saldo.Domain.Saldo saldo = new Saldo.Domain.Saldo(DateTime.Now, 10, 10);
            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
               .ReturnsAsync(saldo);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoDespesaEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoDespesaEvent_DeveRetornarException()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoRepository>()
               .Setup(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()))
               .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoEventHandler.Handle(CriarAtualizarSaldoDespesaEvent(), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialOutraData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ValidarExisteSaldoInicialInferiorOuIgual(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Adicionar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
               .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AtualizarSaldoConsolidadolEvent

        private AtualizarSaldoConsolidadoEvent CriarAtualizarSaldoConsolidadolEvent()
        {
            return new AtualizarSaldoConsolidadoEvent(Guid.NewGuid(), DateTime.Now);
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoConsolidadolEvent_DeveAtualizarComSucesso()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoConsolidadoService>()
                .Setup(x => x.AtualizarSaldos(It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoConsolidadolEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoConsolidadoService>()
                .Verify(x => x.AtualizarSaldos(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoConsolidadolEvent_DeveEnviarEventoNaoAtualizouSaldos()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoConsolidadoService>()
                .Setup(x => x.AtualizarSaldos(It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            // Act
            await saldoEventHandler.Handle(CriarAtualizarSaldoConsolidadolEvent(), It.IsAny<CancellationToken>());

            // Assert
            _mocker.GetMock<ISaldoConsolidadoService>()
                .Verify(x => x.AtualizarSaldos(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoEventHandler_HandleAtualizarSaldoConsolidadolEvent_DeveRetornarException()
        {
            // Arrange
            var saldoEventHandler = CriarSaldoEventHandler();

            _mocker.GetMock<ISaldoConsolidadoService>()
                .Setup(x => x.AtualizarSaldos(It.IsAny<DateTime>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoEventHandler.Handle(CriarAtualizarSaldoConsolidadolEvent(), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoConsolidadoService>()
                .Verify(x => x.AtualizarSaldos(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarEvento(It.IsAny<CancelarLancamentoEvent>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoEventHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion
    }
}
