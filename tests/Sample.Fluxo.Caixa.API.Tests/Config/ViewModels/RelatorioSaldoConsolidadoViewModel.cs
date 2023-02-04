using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Fluxo.Caixa.API.Tests.Config.ViewModels
{
    public class RelatorioSaldoConsolidadoViewModel
    {
        [Column("Data")]
        public string Data { get; set; }
        [Column("Saldo Inicial")]
        public string SaldoInicial { get; set; }
        [Column("Receita")]
        public string Receita { get; set; }
        [Column("Despesa")]
        public string Despesa { get; set; }
        [Column("Saldo Final")]
        public string SaldoFinal { get; set; }
    }
}
