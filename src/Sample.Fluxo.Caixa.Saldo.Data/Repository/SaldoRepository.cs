using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Saldo.Domain;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Fluxo.Caixa.Saldo.Data.Repository
{
    public class SaldoRepository : ISaldoRepository
    {
        private readonly SaldoContext _context;

        public SaldoRepository(SaldoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Domain.Saldo>> ObterListaMaiorIgualData(DateTime dateTime)
        {
            return await _context.Saldos().Find(p => p.DataEscrituracao.Date >= dateTime.Date).ToListAsync();
        }

        public async Task<Domain.Saldo> ObterPorData(DateTime dateTime)
        {
            var result = await ObterTodos(new SaldoFilter() { DataEscrituracao = dateTime.Date });
            return result.Data.FirstOrDefault();
        }

        public async Task<PagedResult<Domain.Saldo>> ObterTodos(SaldoFilter saldoFilter)
        {
            var saldos = await (await _context.Saldos().FindAsync(saldoFilter.Build())).ToListAsync();

            return new PagedResult<Domain.Saldo>()
            {
                Data = saldos,
                TotalResults = saldos.Count(),
                Page = saldoFilter.Page,
                Size = saldoFilter.Size,
            };
        }

        public async Task<bool> ValidarExisteSaldoInicialOutraData(DateTime dateTime)
        {
            return await (await _context.Saldos().FindAsync(n => (n.DataEscrituracao.Year == dateTime.Year &&
                                                n.DataEscrituracao.Day != dateTime.Day) &&
                                                (n.SaldoInicial != 0 ||
                                                n.Receita != 0 ||
                                                n.Despesa != 0))).AnyAsync();
        }

        public async Task<bool> ValidarExisteSaldoInicialInferiorOuIgual(DateTime dateTime)
        {
            return await (await _context.Saldos().FindAsync(n => (n.DataEscrituracao.Year == dateTime.Year &&
                                                            n.DataEscrituracao.Day != dateTime.Day) &&
                                                            (n.SaldoInicial != 0 ||
                                                             n.Receita != 0 ||
                                                             n.Despesa != 0))).AnyAsync();
        }

        public void Adicionar(Domain.Saldo saldo)
        {
            _context.AddCommand(() => _context.Saldos().InsertOneAsync(saldo));
        }

        public void Atualizar(Domain.Saldo saldo)
        {
            _context.AddCommand(() => _context.Saldos().ReplaceOneAsync(Builders<Domain.Saldo>.Filter.Eq("_id", saldo.GetId()), saldo));
        }

        public void Excluir(Guid id)
        {
            _context.AddCommand(() => _context.Saldos().DeleteOneAsync(Builders<Domain.Saldo>.Filter.Eq("_id", id)));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
