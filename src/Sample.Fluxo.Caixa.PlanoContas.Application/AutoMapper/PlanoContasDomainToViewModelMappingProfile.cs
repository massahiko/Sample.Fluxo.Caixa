using AutoMapper;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.AutoMapper
{
    public class PlanoContasDomainToViewModelMappingProfile : Profile
    {
        public PlanoContasDomainToViewModelMappingProfile()
        {
            CreateMap<Conta, ContaViewModel>();
        }
    }
}
