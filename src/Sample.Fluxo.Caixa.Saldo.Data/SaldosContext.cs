using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Sample.Fluxo.Caixa.Core.Data;

namespace Sample.Fluxo.Caixa.Saldo.Data
{
    public class SaldoContext : IUnitOfWork 
    {
        private readonly IMediatorHandler _mediatorHandler;
        private IMongoDatabase Database { get; set; }
        public IClientSessionHandle Session { get; set; }
        public MongoClient MongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly IConfiguration _configuration;

        public SaldoContext(IConfiguration configuration,
                            IMediatorHandler mediatorHandler)
        {
            _configuration = configuration;
            _mediatorHandler = mediatorHandler;

            // Todo comando será armazenado e será processado em SaveChanges
            _commands = new List<Func<Task>>();
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
                return;

            MongoClient = new MongoClient(_configuration["ConnectionStrings:MongoConnection"]);

            Database = MongoClient.GetDatabase(_configuration["ConnectionStrings:MongoDatabaseName"]);
        }

        public IMongoCollection<Domain.Saldo> Saldos()
        {
            ConfigureMongo();

            return Database.GetCollection<Domain.Saldo>(typeof(Domain.Saldo).Name);
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public async Task<bool> Commit()
        {
            try
            {
                ConfigureMongo();

                using (Session = await MongoClient.StartSessionAsync())
                {
                    Session.StartTransaction();

                    var commandTasks = _commands.Select(c => c());

                    await Task.WhenAll(commandTasks);

                    await Session.CommitTransactionAsync();
                }

                var sucesso = _commands.Count > 0;
                if (sucesso) await _mediatorHandler.PublicarEventos(this);

                return sucesso;
            }
            catch (MongoWriteException ex)
            {
                //Falha ocorre porém o registro é persistido normalmente, devido ao tempo foi aplicado esse ajuste técnico ficando como Débito técnico
                //Referência - https://github.com/danielgerlag/workflow-core/issues/1003
                if (ex.Message.Contains("E11000 duplicate key error collection:"))
                    return true;
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}