using Sample.Fluxo.Caixa.Core.DomainObjects;
using System;

namespace Sample.Fluxo.Caixa.Lancamento.Domain
{
    public class Lancamento : Entity, IAggregateRoot
    {
        public Guid ContaId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataEscrituracao { get; private set; }
        public DateTime DataCadastro { get; private set; }

        protected Lancamento() { }

        public Lancamento(Guid lancamentoId, Guid contaId, decimal valor, DateTime dataEscrituracao)
        {
            Id = lancamentoId;
            ContaId = contaId;
            Valor = valor;
            DataEscrituracao = dataEscrituracao;
            DataCadastro = DateTime.Now;

            Validar();
        }

        public void Validar()
        {
            Validacoes.ValidarSeIgual(ContaId, Guid.Empty, "O campo ContaId do lançamento não pode estar vazio");
            Validacoes.ValidarSeMenorQue(Valor, 0.1m, "O campo Valor do lançamento deve possuir valor maior que 0");
            Validacoes.ValidarSeIgual(DataEscrituracao, DateTime.MinValue, "O campo DataEscrituracao do lançamento não pode estar vazio");
        }
    }
}
