using Sample.Fluxo.Caixa.API.Tests.Config;
using Sample.Fluxo.Caixa.Core.Pageable;
using Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
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

    public class ContaControllerTests
    {
        private readonly IntegrationTestsFixture<StartupTests> _testsFixture;

        public ContaControllerTests(IntegrationTestsFixture<StartupTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact, TestPriority(1)]
        public async Task ContaController_AdicionarConta_DeveRetornarComSucesso()
        {
            // Arrange
            var contaViewModel = new ContaViewModel()
            {
                ContaTipo = ContaTipo.SaldoInicial,
                Descricao = "CONTA TESTE INTEGRACAO",
            };

            // Act
            var response = await _testsFixture.Client.PostAsJsonAsync("Conta/Criar", contaViewModel);

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<ContaViewModel>(
                                            await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.Equal(contaViewModel.Descricao, result.Descricao);
        }

        [Fact, TestPriority(2)]
        public async Task ContaController_AtualizarConta_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterTodas");

            response.EnsureSuccessStatusCode();

            var contas = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data;

            var contaAtualizar = contas.FirstOrDefault(n => n.Descricao == "CONTA TESTE INTEGRACAO");

            contaAtualizar.Descricao = "CONTA ATUALIZAR TESTE INTEGRACAO";

            // Act
            response = await _testsFixture.Client.PutAsJsonAsync($"Conta/Editar/{contaAtualizar.Id}", contaAtualizar);

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<ContaViewModel>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.Equal(contaAtualizar.Descricao, result.Descricao);
        }

        [Fact, TestPriority(3)]
        public async Task ContaController_ExcluirConta_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterTodas");

            response.EnsureSuccessStatusCode();

            var contas = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data;
                                        
                            
            var contaExcluir = contas.FirstOrDefault(n => n.Descricao == "CONTA ATUALIZAR TESTE INTEGRACAO");

            // Act
            response = await _testsFixture.Client.DeleteAsync($"Conta/Excluir/{contaExcluir.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<bool>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.True(result);
        }

        [Fact, TestPriority(4)]
        public async Task ContaController_ObterPorId_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterTodas");

            response.EnsureSuccessStatusCode();

            var conta = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data.FirstOrDefault();


            // Act
            response = await _testsFixture.Client.GetAsync($"Conta/ObterPorId/{conta.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<ContaViewModel>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Assert.Equal(conta.Id, result.Id);
        }

        [Fact, TestPriority(5)]
        public async Task ContaController_ObterPorTipo_DeveRetornarComSucesso()
        {
            // Arrange
            var response = await _testsFixture.Client.GetAsync($"Conta/ObterTodas");

            response.EnsureSuccessStatusCode();

            var conta = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data.FirstOrDefault();


            // Act
            response = await _testsFixture.Client.GetAsync($"Conta/ObterPorTipo/{conta.ContaTipo}");

            // Assert
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PagedResult<ContaViewModel>>(
                                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).Data;

            Assert.All(result, result => Assert.Equal(conta.ContaTipo, result.ContaTipo));
        }
    }
}
