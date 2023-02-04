using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sample.Fluxo.Caixa.API.Configuration;
using Sample.Fluxo.Caixa.Lancamento.Application.AutoMapper;
using Sample.Fluxo.Caixa.Lancamento.Application.Commands;
using Sample.Fluxo.Caixa.PlanoContas.Application.AutoMapper;
using Sample.Fluxo.Caixa.Saldo.Application.AutoMapper;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sample.Fluxo.Caixa.API
{
    public class StartupTests
    {
        public IConfiguration Configuration { get; }

        public StartupTests(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApiConfiguration(env);
        }
    }
}
