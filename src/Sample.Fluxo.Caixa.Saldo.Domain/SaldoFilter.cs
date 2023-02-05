using Sample.Fluxo.Caixa.Core.Pageable;
using System.Linq.Expressions;
using System;
using Sample.Fluxo.Caixa.Core.Extensions;

namespace Sample.Fluxo.Caixa.Saldo.Domain
{
    public class SaldoFilter : ByPage
    {
        public Guid Id { get; set; }
        public DateTime? DataEscrituracao { get; set; }
        public DateTime? DataCadastro { get; set; }

        public Expression<Func<Saldo, bool>> Build()
        {
            Expression<Func<Saldo, bool>> filter = x => true;

            if (Id != Guid.Empty)
                filter = filter.And(x => x.Id == Id);

            if (DataEscrituracao.HasValue)
                filter = filter.And(x => x.DataEscrituracao == DataEscrituracao);

            if (DataCadastro.HasValue)
                filter = filter.And(x => x.DataCadastro == DataCadastro);

            return filter;
        }
    }
}
