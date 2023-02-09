
using Bogus;
using Sample.Fluxo.Caixa.API.Tests.Config;
using Sample.Fluxo.Caixa.API.Tests.Config.ViewModels;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.API.Tests.Controllers
{
    [TestCaseOrderer("Features.Tests.PriorityOrderer", "Features.Tests")]
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class SaldoControllerTests
    {
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;
        private List<Tuple<ContaTipo, LancamentoViewModel>> _lancamentosFake;
        private decimal _totalSaldoFake;

        public SaldoControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
            PopularLancamentosFake();
            AtualizarTotalSaldosFake();
        }

        private void AtualizarTotalSaldosFake()
        {
            _totalSaldoFake = 0m;
            _lancamentosFake.ForEach(n =>
            {
                switch (n.Item1)
                {
                    case ContaTipo.SaldoInicial:
                    case ContaTipo.Receita:
                        _totalSaldoFake += n.Item2.Valor;
                        break;
                    case ContaTipo.Despesa:
                        _totalSaldoFake -= n.Item2.Valor;
                        break;
                    default:
                        break;
                }
            });
        }

        private void PopularLancamentosFake()
        {
            _testsFixture.ExcluirTudo();

            _lancamentosFake = (ObterListaLancamentosFake(ContaTipo.SaldoInicial, 1).Result).ToList();
            _lancamentosFake.AddRange(ObterListaLancamentosFake(ContaTipo.Receita, 4).Result);
            _lancamentosFake.AddRange(ObterListaLancamentosFake(ContaTipo.Despesa, 3).Result);

            _testsFixture.AdicionarLancamentos(_lancamentosFake.Select(n => n.Item2));
        }

        private async Task<IEnumerable<Tuple<ContaTipo, LancamentoViewModel>>> ObterListaLancamentosFake(ContaTipo contaTipo, int total = 10)
        {
            var contaId = (await ObterContaPorTipo(contaTipo)).Id;

            return new Faker<Tuple<ContaTipo, LancamentoViewModel>>("pt_BR")
                .CustomInstantiator(f => new Tuple<ContaTipo, LancamentoViewModel>(contaTipo, new LancamentoViewModel()
                {
                    ContaId = contaId,
                    DataEscrituracao = f.Date.Between(DateTime.Now.AddDays(contaTipo == ContaTipo.SaldoInicial ? 0 : 1),
                                                      DateTime.Now.AddDays(contaTipo == ContaTipo.SaldoInicial ? 0 : 6)),
                    Valor = Math.Round(f.Random.Decimal(100, 100), 2),
                })).Generate(total);
        }

        private async Task<ContaViewModel> ObterContaPorTipo(ContaTipo contaTipo)
        {
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterPorTipo/{contaTipo}");

            var conta = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data.FirstOrDefault();

            return conta;
        }

        [Fact, TestPriority(1)]
        public async Task SaldoController_ObterTodos_DeveRetornarComSucesso()
        {
            // Arrange & Act
            var response = await _testsFixture.Client.GetAsync("Saldo/ObterTodos");

            // Assert
            var saldos = JsonSerializer.Deserialize<PagedResult<SaldoViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data;

            var saldoFinal = saldos.OrderByDescending(n => n.DataEscrituracao)
                                .Select(n => n.SaldoFinal)
                                .FirstOrDefault();

            Assert.Equal(_totalSaldoFake, saldoFinal);
        }

        [Fact, TestPriority(2)]
        public async Task SaldoController_GerarRelatorio_DeveRetornarComSucesso()
        {
            // Arrange & Act
            var response = await _testsFixture.Client.GetAsync("Saldo/GerarRelatorio");

            // Assert
            var arquivo = @$"{Path.GetTempPath()}\{Guid.NewGuid()}.csv";
            await HelpersTest.BaixarArquivo(response, arquivo);

            var saldos = CsvHelperTest.ConverterRelatorioParaObjeto<RelatorioSaldoConsolidadoViewModel>(arquivo);
            File.Delete(arquivo);

            var saldoFinal = saldos.OrderByDescending(n => n.Data)
                                .Select(n => n.SaldoFinal)
                                .FirstOrDefault();

            Assert.Equal(_totalSaldoFake, decimal.Parse(saldoFinal));
        }

        [Fact, TestPriority(3)]
        public async Task SaldoController_ObterPorData_DeveRetornarComSucesso()
        {
            // Arrange & Act
            var lancamento = (await ObterListaLancamentosFake(ContaTipo.SaldoInicial, 1)).Select(n => n.Item2);
            _testsFixture.AdicionarLancamentos(lancamento);
            var response = await _testsFixture.Client.GetAsync($"Saldo/ObterPorData/{lancamento.FirstOrDefault().DataEscrituracao.ToString("yyyy-MM-dd")}");

            // Assert
            var saldo = JsonSerializer.Deserialize<SaldoViewModel>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.NotNull(saldo);
        }

        [Fact, TestPriority(4)]
        public async Task SaldoController_Excluir_DeveRetornarComSucesso()
        {
            // Arrange 
            var response = await _testsFixture.Client.GetAsync("Saldo/ObterTodos");

            var saldos = JsonSerializer.Deserialize<PagedResult<SaldoViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data;

            // Act & Assert
            foreach (var saldo in saldos)
            {
                response = await _testsFixture.Client.DeleteAsync($"Saldo/Excluir/{saldo.Id}");

                var result = JsonSerializer.Deserialize<bool>(
                                            await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                Assert.True(result);
            }
        }
    }
}
