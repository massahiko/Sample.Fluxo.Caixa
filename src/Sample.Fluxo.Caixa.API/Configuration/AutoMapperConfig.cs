using Microsoft.Extensions.DependencyInjection;
using Sample.Fluxo.Caixa.Lancamento.Application.AutoMapper;
using Sample.Fluxo.Caixa.PlanoContas.Application.AutoMapper;
using Sample.Fluxo.Caixa.Saldo.Application.AutoMapper;

namespace Sample.Fluxo.Caixa.API.Configuration
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(PlanoContasDomainToViewModelMappingProfile), typeof(PlanoContasViewModelToDomainMappingProfile));
            services.AddAutoMapper(typeof(SaldoDomainToViewModelMappingProfile), typeof(SaldoViewModelToDomainMappingProfile));
            services.AddAutoMapper(typeof(LancamentoDomainToViewModelMappingProfile), typeof(LancamentoViewModelToDomainMappingProfile));
        }
    }
}
