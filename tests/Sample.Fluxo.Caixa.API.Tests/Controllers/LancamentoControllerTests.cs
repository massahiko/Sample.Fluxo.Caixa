using Sample.Fluxo.Caixa.API.Tests.Config;
using Sample.Fluxo.Caixa.API.Tests.Result;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Fluxo.Caixa.API.Tests.Controllers
{
    [TestCaseOrderer("Features.Tests.PriorityOrderer", "Features.Tests")]
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]

    public class LancamentoControllerTests
    {
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

        public LancamentoControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact]
        public async Task LancamentoController_AdicionarLancamento_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterPorTipo/{ContaTipo.Receita.GetHashCode()}");

            var conta = JsonSerializer.Deserialize<IEnumerable<ContaViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                                                            .FirstOrDefault();

            var lancamentoViewModel = new LancamentoViewModel()
            {
                DataEscrituracao = DateTime.Now,
                Valor = 999998,
                ContaId = conta.Id,
            };

            // Act
            response = await _testsFixture.Client.PostAsJsonAsync("Lancamento/Criar", lancamentoViewModel);

            // Assert
            var result = JsonSerializer.Deserialize<LancamentoViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task LancamentoController_AtualizarLancamento_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync("Lancamento/ObterTodos");

            var lancamentos = JsonSerializer.Deserialize<IEnumerable<LancamentoViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            var lancamentoAtualizar = lancamentos.FirstOrDefault();

            lancamentoAtualizar.Valor = 999999;

            // Act
            response = await _testsFixture.Client.PutAsJsonAsync($"Lancamento/Editar/{lancamentoAtualizar.Id}", lancamentoAtualizar);

            // Assert
            var result = JsonSerializer.Deserialize<LancamentoViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.Equal(lancamentoAtualizar.Valor, result.Valor);
        }

        [Fact]
        public async Task LancamentoController_ExcluirLancamento_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterPorTipo/{ContaTipo.Receita.GetHashCode()}");

            var conta = JsonSerializer.Deserialize<IEnumerable<ContaViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                                                            .FirstOrDefault();

            var lancamentoViewModel = new LancamentoViewModel()
            {
                DataEscrituracao = DateTime.Now,
                Valor = 999998,
                ContaId = conta.Id,
            };

            response = await _testsFixture.Client.PostAsJsonAsync("Lancamento/Criar", lancamentoViewModel);

            var lancamento = JsonSerializer.Deserialize<LancamentoViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            // Act
            response = await _testsFixture.Client.DeleteAsync($"Lancamento/Excluir/{lancamento.Id}");

            // Assert
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.True(result);
        }

        [Fact]
        public async Task LancamentoController_ObterPorId_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Lancamento/ObterTodos");

            response.EnsureSuccessStatusCode();

            var lancamento = JsonSerializer.Deserialize<IEnumerable<LancamentoViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                                        .FirstOrDefault();


            // Act
            response = await _testsFixture.Client.GetAsync($"Lancamento/ObterPorId/{lancamento.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<LancamentoViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.Equal(lancamento.Id, result.Id);
        }

        [Fact]
        public async Task LancamentoController_AdicionarLancamentoSaldoInicial_SaldoInicialDataSuperiorInformada()
        {
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterPorTipo/{ContaTipo.SaldoInicial.GetHashCode()}");

            var conta = JsonSerializer.Deserialize<IEnumerable<ContaViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                                                            .FirstOrDefault();

            var lancamentoViewModel = new LancamentoViewModel()
            {
                DataEscrituracao = DateTime.Now,
                Valor = 100,
                ContaId = conta.Id,
            };
            response = await _testsFixture.Client.PostAsJsonAsync("Lancamento/Criar", lancamentoViewModel);

            lancamentoViewModel.DataEscrituracao = lancamentoViewModel.DataEscrituracao.AddDays(+1);

            // Act
            response = await _testsFixture.Client.PostAsJsonAsync("Lancamento/Criar", lancamentoViewModel);

            // Assert

            var mensage = "Existe lançamento de saldo inicial superior a data informada";

            var result = JsonSerializer.Deserialize<ResultError>(await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            Assert.Contains(result.errors.Mensagens, n => n == mensage);
        }
    }
}
