using AutoMapper;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;

namespace Sample.Fluxo.Caixa.Saldo.Application.AutoMapper
{
    public class SaldoDomainToViewModelMappingProfile : Profile
    {
        public SaldoDomainToViewModelMappingProfile()
        {
            CreateMap<Domain.Saldo, SaldoViewModel>();
        }
    }
}
