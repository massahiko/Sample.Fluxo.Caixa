using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.DomainObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Data
{
    public static class MediatorExtension
    {
        public static async Task PublicarEventos(this IMediatorHandler mediator, SaldoContext ctx)
        {
            //var domainEntities = ctx.AddCommand
            //    .Entries<Entity>()
            //    .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            //var domainEvents = domainEntities
            //    .SelectMany(x => x.Entity.Notificacoes)
            //    .ToList();

            //domainEntities.ToList()
            //    .ForEach(entity => entity.Entity.LimparEventos());

            //var tasks = domainEvents
            //    .Select(async (domainEvent) => {
            //        await mediator.PublicarEvento(domainEvent);
            //    });

            //await Task.WhenAll(tasks);
        }
    }
}
