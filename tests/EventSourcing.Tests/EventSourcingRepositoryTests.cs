using EventStore.ClientAPI;
using Moq;
using Moq.AutoMock;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EventSourcing.Tests
{
    public class EventSourcingRepositoryTests
    {
        private readonly AutoMocker _mocker;
        private const string _message = "Falhou";

        public EventSourcingRepositoryTests()
        {
            _mocker = new AutoMocker();
        }

        private EventSourcingRepository CriarEventSourcingRepository()
        {
            return _mocker.CreateInstance<EventSourcingRepository>();
        }

        [Fact]
        public async Task EventSourcingRepository_SalvarEvento_DeveExecutarComSucesso()
        {
            // Arrange
            var eventSourcingRepository = CriarEventSourcingRepository();

            _mocker.GetMock<IEventStoreService>()
                .Setup(x => x.GetConnection())
                .Returns(_mocker.GetMock<IEventStoreConnection>().Object);

            // Act
            await eventSourcingRepository.SalvarEvento(new TesteEvent());

            // Assert
            _mocker.GetMock<IEventStoreService>()
                .Verify(x => x.GetConnection(), Times.Once);
        }

        [Fact]
        public async Task EventSourcingRepository_SalvarEvento_DeveRetornarException()
        {
            // Arrange
            Exception exception = null;
            var eventSourcingRepository = CriarEventSourcingRepository();

            _mocker.GetMock<IEventStoreService>()
                .Setup(x => x.GetConnection())
                .Throws(new Exception(_message));

            // Act
            try
            {
                await eventSourcingRepository.SalvarEvento(new TesteEvent());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(_message, exception.Message);
            _mocker.GetMock<IEventStoreService>().Verify(x => x.GetConnection(), Times.Once);
        }

        [Fact]
        public async Task EventSourcingRepository_ObterEventos_DeveRetornarLista()
        {
            // Arrange
            var eventSourcingRepository = CriarEventSourcingRepository();

            _mocker.GetMock<IEventStoreService>()
                .Setup(x => x.GetConnection())
                .Returns(_mocker.GetMock<IEventStoreConnection>().Object);

            //TODO - não encontrei uma maneira de mockar retorno do ReadStreamEventsForwardAsync, sendo assim ficou esse débito técnico
            //_mocker.GetMock<IEventStoreService>()
            //    .Setup(x => x.GetConnection()
            //                .ReadStreamEventsForwardAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<bool>())
            //    .ReturnsAsync(new StreamEventsSlice
            //    {
            //    });

            // Act
            var result = await eventSourcingRepository.ObterEventos(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            _mocker.GetMock<IEventStoreService>()
                .Verify(x => x.GetConnection(), Times.Once);
        }
    }

    public class TesteEvent : IntegrationEvent
    {
        public TesteEvent()
        {
            AggregateId = Guid.NewGuid();
        }
    }
}
