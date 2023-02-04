using AutoMapper;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;

namespace Sample.Fluxo.Caixa.Saldo.Application.AutoMapper
{
    public class SaldoViewModelToDomainMappingProfile : Profile
    {
        public SaldoViewModelToDomainMappingProfile()
        {
            CreateMap<SaldoViewModel, Domain.Saldo>()
               .ConstructUsing(p =>
                   new Domain.Saldo(
                       p.DataEscrituracao,
                       p.Receita,
                       p.Despesa));
        }
    }
}
