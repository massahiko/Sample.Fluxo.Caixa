using Sample.Fluxo.Caixa.Core.DomainObjects;
using System;

namespace Sample.Fluxo.Caixa.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
