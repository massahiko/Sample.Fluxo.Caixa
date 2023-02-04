using System;

namespace Sample.Fluxo.Caixa.Saldo.Application.ViewModels
{
    public class SaldoViewModel
    {
        public Guid Id { get; set; }
        public DateTime DataEscrituracao { get;  set; }
        public DateTime DataCadastro { get;  set; }
        public decimal SaldoInicial { get; set; }
        public decimal Receita { get;  set; }
        public decimal Despesa { get;  set; }
        public decimal SaldoFinal { get;  set; }

        public static implicit operator string(SaldoViewModel saldoViewModel)
        => $"{saldoViewModel.DataEscrituracao.ToShortDateString()},{saldoViewModel.SaldoInicial},{saldoViewModel.Receita},{saldoViewModel.Despesa},{saldoViewModel.SaldoFinal}";
    }
}
