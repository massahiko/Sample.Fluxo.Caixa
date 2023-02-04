using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Saldo.Application.Events;
using Sample.Fluxo.Caixa.Saldo.Application.Services;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.Saldo.Tests.Application.Services
{
    public class SaldoAppServiceTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public SaldoAppServiceTests()
        {
            _mocker = new AutoMocker();
        }

        private SaldoAppService CriarSaldoAppService()
        {
            return _mocker.CreateInstance<SaldoAppService>();
        }

        private IEnumerable<SaldoViewModel> ObterListaSaldosViewModelFake(int total = 10)
        {
            return new Faker<SaldoViewModel>("pt_BR")
                .CustomInstantiator(f => new SaldoViewModel()
                {
                    Id = Guid.NewGuid(),
                    DataEscrituracao = f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5)),
                    DataCadastro = f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5)),
                    SaldoInicial = f.Random.Decimal(),
                    Receita = f.Random.Decimal(),
                    Despesa = f.Random.Decimal(),
                    SaldoFinal = f.Random.Decimal(),

                }).Generate(total);
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

        #region ObterTodos

        [Fact]
        public async Task SaldoAppService_ObterTodos_DeveRetornarLista()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();
            var saldosFake = ObterListaSaldosFake();
            var saldosFakeViewModel = ObterListaSaldosViewModelFake();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos())
                .ReturnsAsync(saldosFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<IEnumerable<SaldoViewModel>>(saldosFake))
                .Returns(saldosFakeViewModel);

            // Act
            var result = await saldoAppService.ObterTodos();

            // Assert
            Assert.Equal(result, saldosFakeViewModel);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<SaldoViewModel>>(saldosFake), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_ObterTodos_DeveRetornarListaVazia()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos())
                .ReturnsAsync(Enumerable.Empty<Saldo.Domain.Saldo>());

            // Act
            var result = await saldoAppService.ObterTodos();

            // Assert
            Assert.False(result.Any());
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<SaldoViewModel>>(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_ObterTodos_DeveRetornarException()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos())
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoAppService.ObterTodos()
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<SaldoViewModel>>(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ObterPorData

        [Fact]
        public async Task SaldoAppService_ObterPorData_DeveRetornarSaldo()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();
            var saldosFake = ObterListaSaldosFake(1).FirstOrDefault();
            var saldosFakeViewModel = ObterListaSaldosViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
                .ReturnsAsync(saldosFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<SaldoViewModel>(saldosFake))
                .Returns(saldosFakeViewModel);

            // Act
            var result = await saldoAppService.ObterPorData(DateTime.Now);

            // Assert
            Assert.Equal(result, saldosFakeViewModel);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<SaldoViewModel>(saldosFake), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_ObterPorData_DeveRetornarSaldoNulo()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
                .ReturnsAsync(It.IsAny<Saldo.Domain.Saldo>());

            // Act
            var result = await saldoAppService.ObterPorData(DateTime.Now);

            // Assert
            Assert.Null(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_ObterPorData_DeveRetornarException()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterPorData(It.IsAny<DateTime>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoAppService.ObterPorData(DateTime.Now)
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterPorData(It.IsAny<DateTime>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<SaldoViewModel>(It.IsAny<Saldo.Domain.Saldo>()), Times.Never);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ExcluirSaldo

        [Fact]
        public async Task SaldoAppService_Excluir_DeveExcluirComSucesso()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await saldoAppService.ExcluirSaldo(It.IsAny<Guid>());

            // Assert
            Assert.True(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_Excluir_DeveFalhar()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await saldoAppService.ExcluirSaldo(It.IsAny<Guid>());

            // Assert
            Assert.False(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task SaldoAppService_Excluir_DeveRetornarException()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.Excluir(It.IsAny<Guid>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoAppService.ExcluirSaldo(It.IsAny<Guid>())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.Excluir(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region GerarRelatorio

        [Fact]
        public async Task SaldoAppService_GerarRelatorio_DeveRelatorioByteArray()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();
            var saldosFake = ObterListaSaldosFake();
            var saldosFakeViewModel = ObterListaSaldosViewModelFake();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos())
                .ReturnsAsync(saldosFake);

            // Act
            var result = await saldoAppService.GerarRelatorio();

            // Assert
            Assert.NotNull(result);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        [Fact]
        public async Task SaldoAppService_GerarRelatorio_DeveRetornarException()
        {
            // Arrange
            var saldoAppService = CriarSaldoAppService();

            _mocker.GetMock<ISaldoRepository>()
                .Setup(x => x.ObterTodos())
                .Throws(new Exception(_message));
            
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await saldoAppService.GerarRelatorio()
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ISaldoRepository>()
                .Verify(x => x.ObterTodos(), Times.Once);
            _mocker.GetMock<ILogger<SaldoAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(4));
        }

        #endregion
    }
}
