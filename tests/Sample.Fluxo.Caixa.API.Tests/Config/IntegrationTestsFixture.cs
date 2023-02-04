using Microsoft.AspNetCore.Mvc.Testing;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries.ViewModels;
using Sample.Fluxo.Caixa.Saldo.Application.ViewModels;
using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Sample.Fluxo.Caixa.API.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<StartupTests>> { }

    public class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly AppFactory<TStartup> Factory;
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                BaseAddress = new Uri("http://localhost"),
                HandleCookies = true,
                MaxAutomaticRedirections = 7
            };

            Factory = new AppFactory<TStartup>();
            Client = Factory.CreateClient(clientOptions);

            ExcluirTodosSaldos();
            ExcluirTodosLancamentos();
        }

        public void ExcluirTodosLancamentos()
        {
            var response = Client.GetAsync($"Lancamento/ObterTodos").Result;

            response.EnsureSuccessStatusCode();

            var lancamentos = JsonSerializer.Deserialize<IEnumerable<LancamentoViewModel>>(response.Content.ReadAsStringAsync().Result,
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });


            foreach (var item in lancamentos)
            {
                Client.DeleteAsync($"Lancamento/Excluir/{item.Id}").Wait();
            }
        }

        public void ExcluirTodosSaldos()
        {
            var response = Client.GetAsync($"Saldo/ObterTodos").Result;

            response.EnsureSuccessStatusCode();

            var saldos = JsonSerializer.Deserialize<IEnumerable<SaldoViewModel>>(response.Content.ReadAsStringAsync().Result,
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });


            foreach (var item in saldos)
            {
                Client.DeleteAsync($"Saldo/Excluir/{item.Id}").Wait();
            }
        }

        public void AdicionarLancamentos(IEnumerable<LancamentoViewModel> lancamentos)
        {
            foreach (var lancamento in lancamentos)
            {
                Client.PostAsJsonAsync("Lancamento/Criar", lancamento).Wait();            
            }
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
