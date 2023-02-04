using EventSourcing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sample.Fluxo.Caixa.Core.Communication.Mediator;
using Sample.Fluxo.Caixa.Core.Data.EventSourcing;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Lancamento;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.IntegrationEvents.Saldo;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;
using Sample.Fluxo.Caixa.Lancamento.Application.Commands;
using Sample.Fluxo.Caixa.Lancamento.Application.Events;
using Sample.Fluxo.Caixa.Lancamento.Application.Queries;
using Sample.Fluxo.Caixa.Lancamento.Data;
using Sample.Fluxo.Caixa.Lancamento.Data.Repository;
using Sample.Fluxo.Caixa.Lancamento.Domain;
using Sample.Fluxo.Caixa.PlanoContas.Application.Services;
using Sample.Fluxo.Caixa.PlanoContas.Data.Repository;
using Sample.Fluxo.Caixa.Saldo.Application.Events;
using Sample.Fluxo.Caixa.Saldo.Application.Services;
using Sample.Fluxo.Caixa.Saldo.Data;
using Sample.Fluxo.Caixa.Saldo.Data.Repository;
using Sample.Fluxo.Caixa.Saldo.Domain;
using Sample.FluxoCaixa.PlanoContas.Data;
using Sample.FluxoCaixa.PlanoContas.Domain;
using Sample.FluxoCaixa.PlanoContas.Domain.Events;

namespace Sample.Fluxo.Caixa.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Notifications
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Event Sourcing
            services.AddSingleton<IEventStoreService, EventStoreService>();
            services.AddSingleton<IEventSourcingRepository, EventSourcingRepository>();

            // Conta
            services.AddScoped<IContaRepository, ContaRepository>();
            services.AddScoped<IContaAppService, ContaAppService>();
            services.AddScoped<PlanoContasContext>();

            services.AddScoped<INotificationHandler<LancamentoAdicionadoEvent>, LancamentoContaEventHandler>();
            services.AddScoped<INotificationHandler<LancamentoAtualizadoEvent>, LancamentoContaEventHandler>();
            services.AddScoped<INotificationHandler<LancamentoExcluidoEvent>, LancamentoContaEventHandler>();

            // Lançamento
            services.AddScoped<ILancamentoRepository, LancamentoRepository>();
            services.AddScoped<ILancamentoQueries, LancamentoQueries>();
            services.AddScoped<LancamentoContext>();

            services.AddScoped<IRequestHandler<AdicionarLancamentoCommand, bool>, LancamentoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarLancamentoCommand, bool>, LancamentoCommandHandler>();
            services.AddScoped<IRequestHandler<ExcluirLancamentoCommand, bool>, LancamentoCommandHandler>();

            services.AddScoped<INotificationHandler<CancelarLancamentoEvent>, LancamentoEventHandler>();

            // Saldo
            services.AddScoped<ISaldoRepository, SaldoRepository>();
            services.AddScoped<ISaldoConsolidadoService, SaldoConsolidadoService>();
            services.AddScoped<ISaldoAppService, SaldoAppService>();
            services.AddScoped<SaldoContext>();

            services.AddScoped<INotificationHandler<AtualizarSaldoInicialEvent>, SaldoEventHandler>();
            services.AddScoped<INotificationHandler<AtualizarSaldoReceitaEvent>, SaldoEventHandler>();
            services.AddScoped<INotificationHandler<AtualizarSaldoDespesaEvent>, SaldoEventHandler>();
            services.AddScoped<INotificationHandler<AtualizarSaldoConsolidadoEvent>, SaldoEventHandler>();
        }
    }
}
