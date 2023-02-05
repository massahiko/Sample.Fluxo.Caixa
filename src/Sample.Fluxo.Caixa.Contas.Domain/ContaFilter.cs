using Sample.Fluxo.Caixa.Core.Extensions;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sample.Fluxo.Caixa.PlanoContas.Domain
{
    public class ContaFilter : ByPage
    {
        public Guid Id { get; set; }
        public List<string> Descricao { get; set; }
        public bool? Ativo { get; set; }
        public DateTime? DataCadastro { get; set; }
        public ContaTipo ContaTipo { get; set; }
        public EnumTipoFiltroTexto TipoFiltroTexto { get; set; } = EnumTipoFiltroTexto.Equals;

        public Expression<Func<Conta, bool>> Build()
        {
            Expression<Func<Conta, bool>> filter = x => true;

            if (Id != Guid.Empty)
                filter = filter.And(x => x.Id == Id);

            if (Descricao != null && Descricao.Any())
            {
                var index = 0;
                foreach (var descricao in Descricao)
                {
                    if (index == 0)
                    {
                        if (TipoFiltroTexto == EnumTipoFiltroTexto.Equals)
                            filter = filter.And(x => x.Descricao.ToLower().Equals(descricao.ToLower()));
                        else
                            filter = filter.And(x => x.Descricao.ToLower().Contains(descricao.ToLower()));
                    }
                    else
                    {
                        if (TipoFiltroTexto == EnumTipoFiltroTexto.Equals)
                            filter = filter.Or(x => x.Descricao.ToLower().Equals(descricao.ToLower()));
                        else
                            filter = filter.Or(x => x.Descricao.ToLower().Contains(descricao.ToLower()));
                    }

                    index++;
                }
            }

            if (Ativo.HasValue)
                filter = filter.And(x => x.Ativo == Ativo);

            if (DataCadastro.HasValue)
                filter = filter.And(x => x.DataCadastro == DataCadastro);

            if (Enum.IsDefined(typeof(ContaTipo), ContaTipo))
                filter = filter.And(x => x.ContaTipo == ContaTipo);

            return filter;
        }
    }
}
