using AutoMapper;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Data.EventSourcing;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Lancamento.Application.Queries
{
    public class LancamentoQueries : ILancamentoQueries
    {
        private readonly IMapper _mapper;
        private readonly ILogger<LancamentoQueries> _logger;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IEventSourcingRepository _eventSourcingRepository;

        public LancamentoQueries(
            ILogger<LancamentoQueries> logger,
            IMapper mapper,
            ILancamentoRepository lancamentoRepository,
            IEventSourcingRepository eventSourcingRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _lancamentoRepository = lancamentoRepository;
            _eventSourcingRepository = eventSourcingRepository;
        }

        public async Task<LancamentoViewModel> ObterPorId(Guid lancamentoId)
        {
            try
            {
                _logger.LogInformation($"Executando método - LancamentoQueries.ObterPorId");

                var lancamento = await _lancamentoRepository.ObterPorId(lancamentoId);
                if (lancamento == null) return default;

                var result = _mapper.Map<LancamentoViewModel>(lancamento);
                result.HistoricoEventos = await _eventSourcingRepository.ObterEventos(lancamento.Id);
                return result;
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar método - LancamentoQueries.ObterPorId");
                throw;
            }
        }

        public async Task<PagedResult<LancamentoViewModel>> ObterTodos(LancamentoFilter lancamentoFilter)
        {
            try
            {
                _logger.LogInformation($"Executando método - LancamentoQueries.ObterTodos");

                var result = await _lancamentoRepository.ObterTodos(lancamentoFilter);

                return new PagedResult<LancamentoViewModel>()
                {
                    Data = _mapper.Map<IEnumerable<LancamentoViewModel>>(result.Data),
                    TotalResults = result.TotalResults,
                    Page = result.Page,
                    Size = result.Size,
                };
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao executar método - LancamentoQueries.ObterTodos");
                throw;
            }
        }
    }
}
