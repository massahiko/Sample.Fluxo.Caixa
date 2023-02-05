using Sample.Fluxo.Caixa.Core.Data;
using Sample.Fluxo.Caixa.Saldo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ServiceStack;
using System.Drawing.Printing;
using Sample.Fluxo.Caixa.Core.Pageable;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

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
            return _context.Saldos().AsQueryable().Where(n => n.DataEscrituracao >= dateTime);
        }

        public async Task<Domain.Saldo> ObterPorData(DateTime dateTime)
        {
            return _context.Saldos().AsQueryable().Where(n => n.DataEscrituracao.Year == dateTime.Year &&
                                                              n.DataEscrituracao.Month == dateTime.Month &&
                                                              n.DataEscrituracao.Day == dateTime.Day)?.FirstOrDefault();
        }

        public async Task<PagedResult<Domain.Saldo>> ObterTodos(SaldoFilter saldoFilter)
        {
            var saldos = await _context.Saldos().FindAsync(Builders<Domain.Saldo>.Filter.Empty);

            var result = (await saldos.ToListAsync()).Skip(saldoFilter.Skip).Take(saldoFilter.Size);

            return new PagedResult<Domain.Saldo>()
            {
                Data = result,
                TotalResults = result.Count(),
                Page = saldoFilter.Page,
                Size = saldoFilter.Size,
            };
        }

        public async Task<bool> ValidarExisteSaldoInicialOutraData(DateTime dateTime)
        {
            var existe = _context.Saldos().AsQueryable().Where(n => (n.DataEscrituracao.Year == dateTime.Year &&
                                                                       n.DataEscrituracao.Day != dateTime.Day) &&
                                                                       (n.SaldoInicial != 0 ||
                                                                       n.Receita != 0 ||
                                                                       n.Despesa != 0))?.Any();

            return existe.GetValueOrDefault();
        }

        public async Task<bool> ValidarExisteSaldoInicialInferiorOuIgual(DateTime dateTime)
        {
            var existe = _context.Saldos().AsQueryable().Where(n => (n.DataEscrituracao.Year == dateTime.Year &&
                                                                       n.DataEscrituracao.Day != dateTime.Day) &&
                                                                       (n.SaldoInicial != 0 ||
                                                                       n.Receita != 0 ||
                                                                       n.Despesa != 0))?.Any();

            return existe.GetValueOrDefault();
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
