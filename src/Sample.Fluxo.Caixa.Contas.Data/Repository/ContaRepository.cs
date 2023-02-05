using Microsoft.EntityFrameworkCore;
using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Domain;
using Sample.FluxoCaixa.PlanoContas.Data;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
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

        public async Task<PagedResult<Conta>> ObterTodas(ContaFilter contaFilter)
        {
            var contas = await _context.Contas.AsNoTracking()
                .Skip(contaFilter.Skip)
                .Take(contaFilter.Size)
                .Where(contaFilter.Build())
                .ToListAsync();

            return new PagedResult<Conta>()
            {
                Data = contas, 
                TotalResults = await ObterTotal(contaFilter),
                Page = contaFilter.Page,
                Size = contaFilter.Size,
            };
        }

        private async Task<int> ObterTotal(ContaFilter contaFilter)
        {
            return await _context.Contas.AsNoTracking()
                .Where(contaFilter.Build())
                .CountAsync();
        }

        public async Task<Conta> ObterPorId(Guid id)
        {
            return await _context.Contas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}