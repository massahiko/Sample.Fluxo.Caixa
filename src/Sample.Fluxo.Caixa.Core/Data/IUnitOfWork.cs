using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
