using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Collections.Generic;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Bogus;
using Sample.Fluxo.Caixa.Core.Data.EventSourcing;
using System.Linq;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using AutoMapper;

namespace Sample.Fluxo.Caixa.Lancamento.Tests.Application.Queries
{
    public class LancamentoQueriesTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public LancamentoQueriesTests()
        {
            _mocker = new AutoMocker();
        }

        private LancamentoQueries CriarLancamentoQueries()
        {
            return _mocker.CreateInstance<LancamentoQueries>();
        }

        private IEnumerable<LancamentoViewModel> ObterListaLancamentosViewModelFake(int total = 10)
        {
            return new Faker<LancamentoViewModel>("pt_BR")
                .CustomInstantiator(f => new LancamentoViewModel()
                {
                    Id = Guid.NewGuid(),
                    Valor = f.Random.Decimal(1, 100000),
                    DataEscrituracao = f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5)),
                    ContaId = Guid.NewGuid(),
                    HistoricoEventos = new List<StoredEvent>() { new StoredEvent(Guid.NewGuid(), string.Empty, DateTime.Now, string.Empty) }
                }).Generate(total);
        }

        private IEnumerable<Lancamento.Domain.Lancamento> ObterListaLancamentosFake(int total = 10)
        {
            return new Faker<Lancamento.Domain.Lancamento>("pt_BR")
                .CustomInstantiator(f => new Lancamento.Domain.Lancamento(Guid.NewGuid(),
                                                               Guid.NewGuid(),
                                                               f.Random.Decimal(1, 100000),
                                                               f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5))))
                .Generate(total);
        }

        #region ObterPorId

        [Fact]
        public async Task LancamentoQueries_ObterPorId_DeveRetornarLancamento()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();
            var lancamentoFake = ObterListaLancamentosFake(1).FirstOrDefault();
            var lancamentoFakeViewModel = ObterListaLancamentosViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(lancamentoFake);

            _mocker.GetMock<IEventSourcingRepository>()
                .Setup(x => x.ObterEventos(It.IsAny<Guid>()))
                .ReturnsAsync(lancamentoFakeViewModel.HistoricoEventos);

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<LancamentoViewModel>(lancamentoFake))
               .Returns(lancamentoFakeViewModel);

            // Act
            var result = await LancamentoQueries.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.Equal(result, lancamentoFakeViewModel);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IEventSourcingRepository>()
                .Verify(x => x.ObterEventos(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<LancamentoViewModel>(lancamentoFake), Times.Once);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoQueries_ObterPorId_DeveRetornarLancamentoNula()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<Lancamento.Domain.Lancamento>());

            // Act
            var result = await LancamentoQueries.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.Null(result);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IEventSourcingRepository>()
                .Verify(x => x.ObterEventos(It.IsAny<Guid>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<LancamentoViewModel>(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoQueries_ObterPorId_DeveRetornarException()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await LancamentoQueries.ObterPorId(Guid.NewGuid())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IEventSourcingRepository>()
                .Verify(x => x.ObterEventos(It.IsAny<Guid>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<LancamentoViewModel>(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ObterTodos

        [Fact]
        public async Task LancamentoQueries_ObterTodos_DeveRetornarLista()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();
            var lancamentosFake = ObterListaLancamentosFake().OrderBy(p => p.DataEscrituracao);
            var lancamentosFakeViewModel = ObterListaLancamentosViewModelFake().OrderBy(p => p.DataEscrituracao);

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<bool>()))
                .ReturnsAsync(lancamentosFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<IEnumerable<LancamentoViewModel>>(lancamentosFake))
                .Returns(lancamentosFakeViewModel);

            // Act
            var result = await LancamentoQueries.ObterTodos();

            // Assert
            Assert.Equal(result, lancamentosFakeViewModel);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<LancamentoViewModel>>(lancamentosFake), Times.Once);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoQueries_ObterTodos_DeveRetornarListaVazia()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<bool>()))
                .ReturnsAsync(Enumerable.Empty<Lancamento.Domain.Lancamento>());

            // Act
            var result = await LancamentoQueries.ObterTodos();

            // Assert
            Assert.False(result.Any());
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<LancamentoViewModel>>(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task LancamentoQueries_ObterTodas_DeveRetornarException()
        {
            // Arrange
            var LancamentoQueries = CriarLancamentoQueries();

            _mocker.GetMock<ILancamentoRepository>()
                .Setup(x => x.ObterTodos(It.IsAny<bool>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await LancamentoQueries.ObterTodos()
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<ILancamentoRepository>()
                .Verify(x => x.ObterTodos(It.IsAny<bool>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<LancamentoViewModel>>(It.IsAny<Lancamento.Domain.Lancamento>()), Times.Never);
            _mocker.GetMock<ILogger<LancamentoQueries>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion
    }
}
