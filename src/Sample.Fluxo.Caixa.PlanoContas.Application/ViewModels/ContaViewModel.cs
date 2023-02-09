using Sample.FluxoCaixa.PlanoContas.Domain;
using System;
using System.Text.Json.Serialization;

namespace Sample.Fluxo.Caixa.PlanoContas.Application.ViewModels
{
    public class ContaViewModel
    {
        public Guid Id { get; set; }

        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContaTipo ContaTipo { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}
