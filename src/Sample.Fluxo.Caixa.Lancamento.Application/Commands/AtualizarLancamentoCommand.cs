using FluentValidation;
using Sample.Fluxo.Caixa.Core.Messages;
using System;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Commands
{
    public class AtualizarLancamentoCommand : Command
    {
        public Guid LancamentoId { get; private set; }
        public Guid ContaId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataEscrituracao { get; private set; }

        public AtualizarLancamentoCommand(Guid id, Guid contaId, decimal valor, DateTime dataEscrituracao)
        {
            LancamentoId = id;
            ContaId = contaId;
            Valor = valor;
            DataEscrituracao = dataEscrituracao;
        }
        public override bool EhValido()
        {
            ValidationResult = new AtualizarLancamentoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class AtualizarLancamentoValidation : AbstractValidator<AtualizarLancamentoCommand>
    {
        public AtualizarLancamentoValidation()
        {
            RuleFor(c => c.LancamentoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do lançamento é inválido");

            RuleFor(c => c.ContaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da conta é inválido");

            RuleFor(c => c.DataEscrituracao)
               .NotNull()
               .NotEmpty()
               .WithMessage("A Data de escrituração deve ser informada");

            RuleFor(c => c.Valor)
               .GreaterThan(0)
               .WithMessage("O valor do item precisa ser maior que 0");
        }
    }
}
