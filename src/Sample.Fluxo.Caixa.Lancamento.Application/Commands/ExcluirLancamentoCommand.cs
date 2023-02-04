using FluentValidation;
using Sample.Fluxo.Caixa.Core.Messages;
using System;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Commands
{
    public class ExcluirLancamentoCommand : Command
    {
        public Guid LancamentoId { get; private set; }

        public ExcluirLancamentoCommand(Guid id)
        {
            LancamentoId = id;
        }
        public override bool EhValido()
        {
            ValidationResult = new ExcluirLancamentoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class ExcluirLancamentoValidation : AbstractValidator<ExcluirLancamentoCommand>
    {
        public ExcluirLancamentoValidation()
        {
            RuleFor(c => c.LancamentoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do lançamento é inválido");

        }
    }
}
