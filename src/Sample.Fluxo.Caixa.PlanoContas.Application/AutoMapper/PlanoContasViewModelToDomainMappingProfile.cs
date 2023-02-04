using AutoMapper;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.AutoMapper
{
    public class PlanoContasViewModelToDomainMappingProfile : Profile
    {
        public PlanoContasViewModelToDomainMappingProfile()
        {
            CreateMap<ContaViewModel, Conta>()
               .ConstructUsing(p =>
                   new Conta(p.Descricao, 
                             p.Ativo,
                             p.ContaTipo, 
                             p.DataCadastro));
        }
    }
}
