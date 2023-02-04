using System.ComponentModel;

namespace Sample.FluxoCaixa.PlanoContas.Domain
{
    public enum ContaTipo
    {
        [Description("Saldo Inicial")]
        SaldoInicial = 1,
        [Description("Receita")]
        Receita = 2,
        [Description("Despesa")]
        Despesa = 3,
    }
}
