using System;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Domain
{
    public interface ISaldoConsolidadoService
    {
        Task<bool> AtualizarSaldos(DateTime dateTime);
    }
}
