using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Application.Commands;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.Lancamento.Tests.Application.Commands
{
    public class LancamentoCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public LancamentoCommandHandlerTests()
        {
            _mocker = new AutoMocker();
        }

        private LancamentoCommandHandler CriarLancamentoCommandHandler()
        {
            return _mocker.CreateInstance<LancamentoCommandHandler>();
        }

        private Lancamento.Domain.Lancamento CriarLancamento()
        {
            return new Lancamento.Domain.Lancamento(Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
        }

        #region AdicionarLancamentoCommand

        private AdicionarLancamentoCommand CriarAdicionarLancamentoCommand(Guid contaId, decimal valor, DateTime dataEscrituracao)
        {
            return new AdicionarLancamentoCommand(contaId, valor, dataEscrituracao);
        }

        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public async Task LancamentoCommandHandler_HandleAdicionarLancamentoCommand_DeveAdicionarComSucesso(
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarAdicionarLancamentoCommand(contaId, valor, dataEscrituracao), It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoCommandHandler_HandleAdicionarLancamentoCommand_CommandInvalidoRetornaFalse()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarAdicionarLancamentoCommand(Guid.Empty, 0, It.IsAny<DateTime>()), It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Exactly(3));
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public async Task LancamentoCommandHandler_HandleAdicionarLancamentoCommand_DeveRetornarException(
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.Adicionar(It.IsAny<Lancamento.Domain.Lancamento>()))
               .Throws(new Exception(_message));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await lancamentoCommandHandler.Handle(CriarAdicionarLancamentoCommand(contaId, valor, dataEscrituracao), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AtualizarLancamentoCommand

        private AtualizarLancamentoCommand CriarAtualizarLancamentoCommand(Guid contaId, decimal valor, DateTime dataEscrituracao)
        {
            return new AtualizarLancamentoCommand(Guid.NewGuid(), contaId, valor, dataEscrituracao);
        }

        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public async Task LancamentoCommandHandler_HandleAtualizarLancamentoCommand_DeveAtualizarComSucesso(
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
              .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
              .ReturnsAsync(CriarLancamento());

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarAtualizarLancamentoCommand(contaId, valor, dataEscrituracao), It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoCommandHandler_HandleAtualizarLancamentoCommand_CommandInvalidoRetornaFalse()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarAtualizarLancamentoCommand(Guid.Empty, 0, It.IsAny<DateTime>()), It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Exactly(3));
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public async Task LancamentoCommandHandler_HandleAtualizarLancamentoCommand_FalhaAoObterLancamentoRetornaFalse(
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarAtualizarLancamentoCommand(contaId, valor, dataEscrituracao), It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Theory]
        [InlineData("86040cc2-ce8f-465f-8b50-bdfc1883a02c", 100, "2023-01-01")]
        public async Task LancamentoCommandHandler_AtualizarLancamentoCommand_DeveRetornarException(
            Guid contaId,
            decimal valor,
            DateTime dataEscrituracao)
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
               .Throws(new Exception(_message));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await lancamentoCommandHandler.Handle(CriarAtualizarLancamentoCommand(contaId, valor, dataEscrituracao), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }


        #endregion

        #region ExcluirLancamentoCommand

        private ExcluirLancamentoCommand CriarExcluirLancamentoCommand(Guid lancamentoId)
        {
            return new ExcluirLancamentoCommand(lancamentoId);
        }

        [Fact]
        public async Task LancamentoCommandHandler_HandleExcluirLancamentoCommand_DeveExcluirComSucesso()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
              .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
              .ReturnsAsync(CriarLancamento());

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarExcluirLancamentoCommand(Guid.NewGuid()), It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoCommandHandler_HandleExcluirLancamentoCommand_CommandInvalidoRetornaFalse()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarExcluirLancamentoCommand(Guid.Empty), It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoCommandHandler_HandleExcluirLancamentoCommand_FalhaAoObterLancamentoRetornaFalse()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            // Act
            var result = await lancamentoCommandHandler.Handle(CriarExcluirLancamentoCommand(Guid.NewGuid()), It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ILancamentoRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoCommandHandler_ExcluirLancamentoCommand_DeveRetornarException()
        {
            // Arrange
            var lancamentoCommandHandler = CriarLancamentoCommandHandler();

            _mocker.GetMock<ILancamentoRepository>()
               .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
               .Throws(new Exception(_message));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await lancamentoCommandHandler.Handle(CriarExcluirLancamentoCommand(Guid.NewGuid()), It.IsAny<CancellationToken>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<LancamentoCommandHandler>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion
    }
}
