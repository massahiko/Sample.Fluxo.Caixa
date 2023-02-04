﻿using AutoMapper;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;

namespace Sample.Fluxo.Caixa.Lancamento.Application.AutoMapper
{
    public class LancamentoViewModelToDomainMappingProfile : Profile
    {
        public LancamentoViewModelToDomainMappingProfile()
        {
            CreateMap<LancamentoViewModel, Domain.Lancamento>();
        }
    }
}
