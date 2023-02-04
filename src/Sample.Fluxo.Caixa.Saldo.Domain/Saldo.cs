using Sample.Fluxo.Caixa.Core.DomainObjects;
using System;
using System.Drawing;

namespace Sample.Fluxo.Caixa.Saldo.Domain
{
    public class Saldo : Entity, IAggregateRoot
    {
        public decimal SaldoInicial { get; private set; }
        public DateTime DataEscrituracao { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public decimal Receita { get; private set; }
        public decimal Despesa { get; private set; }
        public decimal SaldoFinal { get; private set; }

        protected Saldo() { }

        public Saldo(DateTime dataEscrituracao, decimal receita, decimal despesa)
        {
            DataEscrituracao = new DateTime(dataEscrituracao.Year, dataEscrituracao.Month, dataEscrituracao.Day, 0, 0, 0);
            DataCadastro = DateTime.Now;
            Receita = receita;
            Despesa = despesa;

            AtualizarSaldoFinal();
        }

        public void AtualizarSaldoInicial(decimal valor, bool acumular = true)
        {
            if (!acumular)
                SaldoInicial = 0;
            SaldoInicial += valor;
            AtualizarSaldoFinal();
        }

        public void AtualizarReceita(decimal valor)
        {
            Receita += valor;
            AtualizarSaldoFinal();
        }

        public void AtualizarDespesa(decimal valor)
        {
            Despesa += valor;
            AtualizarSaldoFinal();
        }

        private void AtualizarSaldoFinal()
        {
            SaldoFinal = (SaldoInicial + Receita - Despesa);
        }
    }
}
