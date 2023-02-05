using Sample.Fluxo.Caixa.Core.Extensions;
using Sample.Fluxo.Caixa.Core.Pageable;
using System;
using System.Linq.Expressions;

namespace Sample.Fluxo.Caixa.Lancamento.Domain
{
    public class LancamentoFilter : ByPage
    {
        public Guid Id { get; set; }
        public Guid ContaId { get; set; }
        public DateTime? DataEscrituracao { get; set; }
        public DateTime? DataCadastro { get; set; }

        public Expression<Func<Lancamento, bool>> Build()
        {
            Expression<Func<Lancamento, bool>> filter = x => true;

            if (Id != Guid.Empty)
                filter = filter.And(x => x.Id == Id);

            if (ContaId != Guid.Empty)
                filter = filter.And(x => x.ContaId == ContaId);

            if (DataEscrituracao.HasValue)
                filter = filter.And(x => x.DataCadastro == DataCadastro);

            if (DataCadastro.HasValue)
                filter = filter.And(x => x.DataCadastro == DataCadastro);

            return filter;
        }
    }
}
