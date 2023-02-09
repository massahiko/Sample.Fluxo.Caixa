using AutoMapper;
using Microsoft.Extensions.Logging;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.Fluxo.Caixa.PlanoContas.Domain;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.Services
{
    public class ContaAppService : IContaAppService
    {
        private readonly ILogger<ContaAppService> _logger;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IContaRepository _contaRepository;
        private readonly IMapper _mapper;

        public ContaAppService(
            ILogger<ContaAppService> logger,
            IMediatorHandler mediatorHandler,
            IContaRepository contaRepository,
            IMapper mapper)
        {
            _mediatorHandler = mediatorHandler;
            _logger = logger;
            _contaRepository = contaRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ContaViewModel>> ObterPorTipo(ContaTipo contaTipo)
        {
            try
            {
                _logger.LogInformation($"Obtendo contas por tipo: {contaTipo}");

                return await ObterTodas(new ContaFilter { ContaTipo = contaTipo });
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao obter contas por tipo");
                throw;
            }
        }

        public async Task<ContaViewModel> ObterPorId(Guid id)
        {
            try
            {
                _logger.LogInformation($"Obtendo contas por id: {id}");

                var conta = await _contaRepository.ObterPorId(id);
                if (conta == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Conta", "Conta não encontrada!"));
                    return default;
                }
                return _mapper.Map<ContaViewModel>(conta);
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao obter contas por id");
                throw;
            }
        }

        public async Task<PagedResult<ContaViewModel>> ObterTodas(ContaFilter contaFilter)
        {
            try
            {
                _logger.LogInformation($"Obtendo lista de contas");

                var result = await _contaRepository.ObterTodas(contaFilter);

                return new PagedResult<ContaViewModel>()
                {
                    Data = _mapper.Map<IEnumerable<ContaViewModel>>(result.Data),
                    TotalResults = result.TotalResults,
                    Page = result.Page,
                    Size = result.Size,
                };
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao obter lista de contas");
                throw;
            }
        }

        public async Task<ContaViewModel> AdicionarConta(ContaViewModel contaViewModel)
        {
            try
            {
                _logger.LogInformation($"Adicionando conta");

                var conta = _mapper.Map<Conta>(contaViewModel);
                _contaRepository.Adicionar(conta);

                var sucesso = await _contaRepository.UnitOfWork.Commit();
                if (!sucesso)
                    return default;

                return _mapper.Map<ContaViewModel>(conta);
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao adicionar conta");
                throw;
            }
        }

        public async Task<ContaViewModel> AtualizarConta(ContaViewModel contaViewModel)
        {
            try
            {
                _logger.LogInformation($"Atualizando conta");

                var conta = _mapper.Map<Conta>(contaViewModel);
                _contaRepository.Atualizar(conta);

                var sucesso = await _contaRepository.UnitOfWork.Commit();
                if (!sucesso)
                    return default;

                return _mapper.Map<ContaViewModel>(conta);
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao atualizar conta");
                throw;
            }
        }


        public async Task<bool> ExcluirConta(Guid contaId)
        {
            try
            {
                _logger.LogInformation($"Atualizando conta");

                var conta = await _contaRepository.ObterPorId(contaId);
                if (conta == null)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification("Conta", "Conta não encontrada!"));
                    return false;
                }
                _contaRepository.Excluir(conta);

                return await _contaRepository.UnitOfWork.Commit();
            }
            catch (Exception)
            {
                _logger.LogError($"Falha ao excluir conta");
                throw;
            }
        }

        public void Dispose()
        {
            _contaRepository?.Dispose();
        }
    }
}
