using Bogus;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Domain
{
    public class SaldoConsolidadoServiceTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public SaldoConsolidadoServiceTests()
        {
            _mocker = new AutoMocker();
        }

        private SaldoConsolidadoService CriarSaldoConsolidadoService()
        {
            return _mocker.CreateInstance<SaldoConsolidadoService>();
        }

        private IEnumerable<Saldo.Domain.Saldo> ObterListaSaldosFake(int total = 10)
        {
            return new Faker<Saldo.Domain.Saldo>("pt_BR")
                .CustomInstantiator(f => new Saldo.Domain.Saldo(f.Date.Between(DateTime.Now.AddDays(-1),
                                                                               DateTime.Now.AddDays(-5)),
                                                                f.Random.Decimal(),
                                                                f.Random.Decimal()))
                .Generate(total);
        }

        [Fact]
        public async Task SaldoConsolidadoService_AtualizarSaldos_DeveAtualizarComSucesso()
        {
            // Arrange
            var saldoConsolidadoService = CriarSaldoConsolidadoService();
            var saldosFake = ObterListaSaldosFake();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<SaldoFilter>()))
                .ReturnsAsync(new Core.Pageable.PagedResult<Saldo.Domain.Saldo>()
                {
                    Data = saldosFake
                });

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await saldoConsolidadoService.AtualizarSaldos(DateTime.Now);

            // Assert
            Assert.True(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<SaldoFilter>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Exactly(saldosFake.Count() - 1));
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
        }

        [Fact]
        public async Task SaldoConsolidadoService_AtualizarSaldos_NaoAtualizarExisteApenasUmRegistro()
        {
            // Arrange
            var saldoConsolidadoService = CriarSaldoConsolidadoService();
            var saldosFake = ObterListaSaldosFake(1);

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<SaldoFilter>()))
                .ReturnsAsync(new Core.Pageable.PagedResult<Saldo.Domain.Saldo>() { Data = saldosFake });

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await saldoConsolidadoService.AtualizarSaldos(DateTime.Now);

            // Assert
            Assert.True(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<SaldoFilter>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
        }

        [Fact]
        public async Task SaldoConsolidadoService_AtualizarSaldos_DeveNotificar()
        {
            // Arrange
            var saldoConsolidadoService = CriarSaldoConsolidadoService();
            var saldosFake = ObterListaSaldosFake();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<SaldoFilter>()))
                .ReturnsAsync(new Core.Pageable.PagedResult<Saldo.Domain.Saldo>()
                {
                    Data = saldosFake
                });

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await saldoConsolidadoService.AtualizarSaldos(DateTime.Now);

            // Assert
            Assert.False(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<SaldoFilter>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Exactly(saldosFake.Count() - 1));
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);

            //Valida Notificação
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
        }

        [Fact]
        public async Task SaldoConsolidadoService_AtualizarSaldos_DeveRetornarException()
        {
            // Arrange
            var saldoConsolidadoService = CriarSaldoConsolidadoService();

            // Act
            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<SaldoFilter>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoConsolidadoService.AtualizarSaldos(DateTime.Now)
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<SaldoFilter>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
        }
    }
}
