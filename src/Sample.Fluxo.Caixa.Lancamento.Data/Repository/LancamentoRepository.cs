using Microsoft.EntityFrameworkCore;
using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Domain.Lancamento>> ObterTodos(bool asNoTracking = false)
        {
            if (asNoTracking)
                return await _context.Lancamentos.AsNoTracking().ToListAsync();

            return await _context.Lancamentos.ToListAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
