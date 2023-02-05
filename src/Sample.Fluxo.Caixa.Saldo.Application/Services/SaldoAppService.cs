using AutoMapper;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Application.Services
{
    public class SaldoAppService : ISaldoAppService
    {
        private readonly ILogger<SaldoAppService> _logger;
        private readonly ISaldoRepository _saldoRepository;
        private readonly IMapper _mapper;

        public SaldoAppService(
            ILogger<SaldoAppService> logger,
            ISaldoRepository saldoRepository,
            IMapper mapper)
        {
            _logger = logger;
            _saldoRepository = saldoRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SaldoViewModel>> ObterTodos(SaldoFilter saldoFilter)
        {
            try
            {
                _logger.LogInformation($"Obtendo lista de saldos");

                var result = await _saldoRepository.ObterTodos(saldoFilter);

                return new PagedResult<SaldoViewModel>()
                {
                    Data = _mapper.Map<IEnumerable<SaldoViewModel>>(result.Data),
                    TotalResults = result.TotalResults,
                    Page = result.Page,
                    Size = result.Size,
                };
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao obter lista de saldos");
                throw;
            }
        }

        public async Task<SaldoViewModel> ObterPorData(DateTime dateTime)
        {
            try
            {
                _logger.LogInformation($"Obtendo saldo por data: {dateTime}");

                return _mapper.Map<SaldoViewModel>(await _saldoRepository.ObterPorData(dateTime));
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao obter saldo por data");
                throw;
            }
        }

        public async Task<bool> ExcluirSaldo(Guid id)
        {
            try
            {
                _logger.LogInformation($"Excluindo Saldo");

                _saldoRepository.Excluir(id);
                return await _saldoRepository.UnitOfWork.Commit();
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao excluir saldo");
                throw;
            }
        }

        public async Task<byte[]> GerarRelatorio()
        {
            try
            {
                _logger.LogInformation($"Gerando relatório de Saldos Consolidados");

                var saldos = (await ObterTodos(new SaldoFilter())).Data;

                var relatorio = new List<string>()
                {
                    "Data,SaldoInicial,Receita,Despesa,SaldoFinal"
                };

                relatorio.AddRange(saldos.Select(saldo => (string)saldo));

                var dataAsBytes = relatorio.SelectMany(s => System.Text.Encoding.UTF8.GetBytes($"{s}{Environment.NewLine}")).ToArray();

                return dataAsBytes;
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao gerar relatórios de Saldos Consolidados");
                throw;
            }
        }
    }
}
