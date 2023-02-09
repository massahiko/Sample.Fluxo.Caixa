using Microsoft.EntityFrameworkCore;
using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Lancamento.Data.Repository
{
    public class LancamentoRepository : ILancamentoRepository
    {
        private readonly LancamentoContext _context;

        public LancamentoRepository(LancamentoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Adicionar(Domain.Lancamento lancamento)
        {
            _context.Lancamentos.Add(lancamento);
        }

        public void Atualizar(Domain.Lancamento lancamento)
        {
            _context.Lancamentos.Update(lancamento);
        }

        public void Excluir(Domain.Lancamento lancamento)
        {
            _context.Lancamentos.Remove(lancamento);
        }

        public async Task<Domain.Lancamento> ObterPorId(Guid id, bool asNoTracking = false)
        {
            if (asNoTracking)
                return await _context.Lancamentos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            return await _context.Lancamentos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<Domain.Lancamento>> ObterTodos(LancamentoFilter lancamentoFilter, bool asNoTracking = false)
        {
            var lancamentos = Enumerable.Empty<Domain.Lancamento>();

            if (asNoTracking)
            {
                lancamentos = await _context.Lancamentos.AsNoTracking()
                                .Skip(lancamentoFilter.Skip)
                                .Take(lancamentoFilter.Size)
                                .Where(lancamentoFilter.Build())
                                .ToListAsync();
            }
            else
            {
                lancamentos = await _context.Lancamentos
                                .Skip(lancamentoFilter.Skip)
                                .Take(lancamentoFilter.Size)
                                .Where(lancamentoFilter.Build())
                                .ToListAsync();
            }

            return new PagedResult<Domain.Lancamento>()
            {
                Data = lancamentos,
                TotalResults = await ObterTotal(lancamentoFilter),
                Page = lancamentoFilter.Page,
                Size = lancamentoFilter.Size,
            };
        }

        private async Task<int> ObterTotal(LancamentoFilter lancamentoFilter)
        {
            return await _context.Lancamentos.AsNoTracking()
                .Where(lancamentoFilter.Build())
                .CountAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
