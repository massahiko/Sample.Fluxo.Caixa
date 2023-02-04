using AutoMapper;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;

namespace Sample.Fluxo.Caixa.Lancamento.Application.AutoMapper
{
    public class LancamentoDomainToViewModelMappingProfile : Profile
    {
        public LancamentoDomainToViewModelMappingProfile()
        {
            CreateMap<Domain.Lancamento, LancamentoViewModel>();
        }
    }
}
