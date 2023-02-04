using Microsoft.EntityFrameworkCore;
using Sample.Fluxo.Caixa.Core.Data;
using Sample.FluxoCaixa.PlanoContas.Data;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.PlanoContas.Data.Repository
{
    public class ContaRepository : IContaRepository
    {
        private readonly PlanoContasContext _context;

        public ContaRepository(PlanoContasContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Adicionar(Conta conta)
        {
            _context.Contas.Add(conta);
        }

        public void Atualizar(Conta conta)
        {
            _context.Contas.Update(conta);
        }

        public void Excluir(Conta conta)
        {
            _context.Contas.Remove(conta);
        }

        public async Task<IEnumerable<Conta>> ObterTodas()
        {
            return await _context.Contas.AsNoTracking().ToListAsync();
        }

        public async Task<Conta> ObterPorId(Guid id)
        {
            return await _context.Contas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Conta>> ObterPorTipo(ContaTipo contaTipo)
        {
            return await _context.Contas.AsNoTracking().Where(c => c.ContaTipo == contaTipo).ToListAsync();

        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}