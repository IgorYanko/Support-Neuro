using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NeuroApp.Classes
{
    public enum SaleType
    {
        [JsonPropertyName("Venda")]
        Venda,

        [JsonPropertyName("Assistência Técnica")]
        AssistênciaTécnica,

        [JsonPropertyName("Orçamento")]
        Orçamento,

        [JsonPropertyName("Compra")]
        Compra,

        [JsonPropertyName("Venda Consignada")]
        VendaConsignada,

        [JsonPropertyName("Venda Representação")]
        VendaRepresentação,

        [JsonPropertyName("Bonificação/Remessa")]
        BonificaçãoRemessa,

        [JsonPropertyName("Ordem de Serviço")]
        OrdemServiço,

        [JsonPropertyName("Transferência")]
        Transferência,

        Unknown
    }

    public enum Status
    {
        [JsonPropertyName("Em aberto")]
        Emaberto,

        [JsonPropertyName("Em orçamento")]
        Emorçamento,

        [JsonPropertyName("Aprovado")]
        Aprovado,
        
        [JsonPropertyName("Faturado")]
        Faturado,

        [JsonPropertyName("Orçamento Recusado")]
        OrçamentoRecusado,

        [JsonPropertyName("Cancelado")]
        Cancelado
    }

    public enum NfeStatus
    {
        Autorizada,
        Pendente,
        Cancelada,
        Denegada
    }

    public class Sales
    {
        [JsonPropertyName("_id")]
        public string _id {  get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("dateCreated")]
        [JsonConverter(typeof(DateConverter))]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("customerId")]
        public string customerId { get; set; }

        [JsonPropertyName("personName")]
        public string? personName { get; set; }

        [JsonPropertyName("personRazao")]
        public string? personRazao { get; set; }

        [JsonConverter(typeof(CustomEnumConverter<SaleType>))]
        public SaleType Type { get; set; } = SaleType.Unknown;

        [JsonConverter(typeof(CustomEnumConverter<Status>))]
        public Status status { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }

    public class Tag
    {
        [JsonPropertyName("tagId")]
        public string TagId { get; set; }

        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("textColor")]
        public string TextColor { get; set; }
    }
}
