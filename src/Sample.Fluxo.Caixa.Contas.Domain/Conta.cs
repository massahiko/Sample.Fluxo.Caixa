using Sample.Fluxo.Caixa.Core.DomainObjects;
using System;

namespace Sample.FluxoCaixa.PlanoContas.Domain
{
    public class Conta : Entity, IAggregateRoot
    {
        public string Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public ContaTipo ContaTipo { get; private set; }

        protected Conta() { }

        public Conta(string descricao, bool ativo, ContaTipo contaTipo, DateTime dataCadastro)
        {
            Descricao = descricao;
            Ativo = ativo;
            ContaTipo = contaTipo;
            DataCadastro = dataCadastro;

            Validar();
        }

        public void Validar()
        {
            Validacoes.ValidarSeVazio(Descricao, "O campo Descrição da Conta não pode estar vazio");
        }
    }
}
