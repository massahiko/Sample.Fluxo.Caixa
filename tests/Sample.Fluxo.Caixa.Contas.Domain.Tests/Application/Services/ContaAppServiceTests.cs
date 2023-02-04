using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.DomainObjects;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.PlanoContas.Application.Services;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.PlanoContas.Tests.Application.Services
{
    public class ContaAppServiceTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public ContaAppServiceTests()
        {
            _mocker = new AutoMocker();
        }

        private ContaAppService CriarContaAppService()
        {
            return _mocker.CreateInstance<ContaAppService>();
        }

        private IEnumerable<ContaViewModel> ObterListaContasViewModelFake(int total = 10)
        {
            return new Faker<ContaViewModel>("pt_BR")
                .CustomInstantiator(f => new ContaViewModel()
                {
                    Id = Guid.NewGuid(),
                    Descricao = f.Name.FullName(),
                    Ativo = true,
                    ContaTipo = f.PickRandom<ContaTipo>(),
                    DataCadastro = f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5)),

                }).Generate(total);
        }

        private IEnumerable<Conta> ObterListaContasFake(int total = 10)
        {
            return new Faker<Conta>("pt_BR")
                .CustomInstantiator(f => new Conta(f.Name.FullName(),
                                                   true,
                                                   f.PickRandom<ContaTipo>(),
                                                   f.Date.Between(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-5))))
                .Generate(total);
        }

        #region ObterPorTipo

        [Fact]
        public async Task ContaAppService_ObterPorTipo_DeveRetornarLista()
        {
            // Arrange
            var contaAppService = CriarContaAppService();
            var contasFake = ObterListaContasFake(10);
            var contasFakeViewModel = ObterListaContasViewModelFake();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorTipo(It.IsAny<ContaTipo>()))
                .ReturnsAsync(contasFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<IEnumerable<ContaViewModel>>(contasFake))
                .Returns(contasFakeViewModel);

            // Act
            var result = await contaAppService.ObterPorTipo(ContaTipo.Receita);

            // Assert
            Assert.Equal(result, contasFakeViewModel);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorTipo(It.IsAny<ContaTipo>()), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<ContaViewModel>>(contasFake), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterPorTipo_DeveRetornarListaVazia()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorTipo(It.IsAny<ContaTipo>()))
                .ReturnsAsync(Enumerable.Empty<Conta>());

            // Act
            var result = await contaAppService.ObterPorTipo(ContaTipo.Receita);

            // Assert
            Assert.False(result.Any());
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorTipo(It.IsAny<ContaTipo>()), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<ContaViewModel>>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterPorTipo_DeveRetornarException()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorTipo(It.IsAny<ContaTipo>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await contaAppService.ObterPorTipo(ContaTipo.Receita)
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorTipo(It.IsAny<ContaTipo>()), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<ContaViewModel>>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ObterPorId

        [Fact]
        public async Task ContaAppService_ObterPorId_DeveRetornarConta()
        {
            // Arrange
            var contaAppService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();
            var contaFakeViewModel = ObterListaContasViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(contaFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<ContaViewModel>(contaFake))
                .Returns(contaFakeViewModel);

            // Act
            var result = await contaAppService.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.Equal(result, contaFakeViewModel);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(contaFake), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterPorId_DeveRetornarContaNula()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(It.IsAny<Conta>());

            // Act
            var result = await contaAppService.ObterPorId(Guid.NewGuid());

            // Assert
            Assert.Null(result);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterPorId_DeveRetornarException()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await contaAppService.ObterPorId(Guid.NewGuid())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ObterTodas

        [Fact]
        public async Task ContaAppService_ObterTodas_DeveRetornarLista()
        {
            // Arrange
            var contaAppService = CriarContaAppService();
            var contasFake = ObterListaContasFake();
            var contasFakeViewModel = ObterListaContasViewModelFake();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterTodas())
                .ReturnsAsync(contasFake);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<IEnumerable<ContaViewModel>>(contasFake))
                .Returns(contasFakeViewModel);

            // Act
            var result = await contaAppService.ObterTodas();

            // Assert
            Assert.Equal(result, contasFakeViewModel);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterTodas(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<ContaViewModel>>(contasFake), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterTodas_DeveRetornarListaVazia()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterTodas())
                .ReturnsAsync(Enumerable.Empty<Conta>());

            // Act
            var result = await contaAppService.ObterTodas();

            // Assert
            Assert.False(result.Any());
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterTodas(), Times.Once);
            _mocker.GetMock<IMapper>()
                 .Verify(x => x.Map<IEnumerable<ContaViewModel>>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ObterTodas_DeveRetornarException()
        {
            // Arrange
            var contaAppService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.ObterTodas())
                .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                 await contaAppService.ObterTodas()
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.ObterTodas(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<IEnumerable<ContaViewModel>>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
               .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                   It.IsAny<EventId>(),
                                   It.Is<It.IsAnyType>((v, t) => true),
                                   It.IsAny<Exception>(),
                                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AdicionarConta

        [Fact]
        public async Task ContaAppService_AdicionarConta_DeveAdicionarComSucesso()
        {
            // Arrange
            var contaEventService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();
            var contaFakeViewModel = ObterListaContasViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(contaFakeViewModel))
               .Returns(contaFake);

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<ContaViewModel>(contaFake))
               .Returns(contaFakeViewModel);

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await contaEventService.AdicionarConta(contaFakeViewModel);

            // Assert
            Assert.Equal(result, contaFakeViewModel);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_AdicionarConta_FalhaAoSalvarConta()
        {
            // Arrange
            var contaEventService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();
            var contaFakeViewModel = ObterListaContasViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(contaFakeViewModel))
               .Returns(contaFake);

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<ContaViewModel>(contaFake))
               .Returns(contaFakeViewModel);

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await contaEventService.AdicionarConta(contaFakeViewModel);

            // Assert
            Assert.Null(result);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_AdicionarConta_DeveRetornarException()
        {
            // Arrange
            var contaEventService = CriarContaAppService();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(It.IsAny<ContaViewModel>()))
               .Throws(new Exception(_message));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
               await contaEventService.AdicionarConta(ObterListaContasViewModelFake(1).FirstOrDefault())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Adicionar(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region AtualizarConta

        [Fact]
        public async Task ContaAppService_AtualizarConta_DeveAtualizarComSucesso()
        {
            // Arrange
            var contaEventService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();
            var contaFakeViewModel = ObterListaContasViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(contaFakeViewModel))
               .Returns(contaFake);

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<ContaViewModel>(contaFake))
               .Returns(contaFakeViewModel);

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await contaEventService.AtualizarConta(contaFakeViewModel);

            // Assert
            Assert.Equal(result, contaFakeViewModel);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_AtualizarConta_FalhaAoSalvarConta()
        {
            // Arrange
            var contaEventService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();
            var contaFakeViewModel = ObterListaContasViewModelFake(1).FirstOrDefault();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(contaFakeViewModel))
               .Returns(contaFake);

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<ContaViewModel>(contaFake))
               .Returns(contaFakeViewModel);

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await contaEventService.AtualizarConta(contaFakeViewModel);

            // Assert
            Assert.Null(result);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_AtualizarConta_DeveRetornarException()
        {
            // Arrange
            var contaEventService = CriarContaAppService();

            _mocker.GetMock<IMapper>()
               .Setup(x => x.Map<Conta>(It.IsAny<ContaViewModel>()))
               .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
               await contaEventService.AtualizarConta(ObterListaContasViewModelFake(1).FirstOrDefault())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Atualizar(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<ContaViewModel>(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IMapper>()
                .Verify(x => x.Map<Conta>(It.IsAny<ContaViewModel>()), Times.Once);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion

        #region ExcluirConta

        [Fact]
        public async Task ContaAppService_ExcluirConta_DeveExcluirComSucesso()
        {
            // Arrange
            var contaEventService = CriarContaAppService();
            var contaFake = ObterListaContasFake(1).FirstOrDefault();

            _mocker.GetMock<IContaRepository>()
               .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
               .ReturnsAsync(contaFake);

            _mocker.GetMock<IContaRepository>()
                .Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await contaEventService.ExcluirConta(Guid.NewGuid());

            // Assert
            Assert.True(result);
            _mocker.GetMock<IContaRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Excluir(It.IsAny<Conta>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ExcluirConta_FalhaAoSalvarConta()
        {
            // Arrange
            var contaEventService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
               .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
               .ReturnsAsync(It.IsAny<Conta>());

            // Act
            var result = await contaEventService.ExcluirConta(Guid.NewGuid());

            // Assert
            Assert.False(result);
            _mocker.GetMock<IContaRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Excluir(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Once);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task ContaAppService_ExcluirConta_DeveRetornarException()
        {
            // Arrange
            var contaEventService = CriarContaAppService();

            _mocker.GetMock<IContaRepository>()
               .Setup(x => x.ObterPorId(It.IsAny<Guid>()))
               .Throws(new Exception(_message));

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
               await contaEventService.ExcluirConta(Guid.NewGuid())
            );

            // Assert
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IContaRepository>()
               .Verify(x => x.ObterPorId(It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()), Times.Never);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.Excluir(It.IsAny<Conta>()), Times.Never);
            _mocker.GetMock<IContaRepository>()
                .Verify(x => x.UnitOfWork.Commit(), Times.Never);
            _mocker.GetMock<ILogger<ContaAppService>>()
                .Verify(x => x.Log(It.IsAny<LogLevel>(),
                                    It.IsAny<EventId>(),
                                    It.Is<It.IsAnyType>((v, t) => true),
                                    It.IsAny<Exception>(),
                                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        }

        #endregion
    }
}
