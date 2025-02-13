using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Media;

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

        [JsonPropertyName("Locação")]
        Locação,

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
        Cancelado,

        [JsonPropertyName("Recusado")]
        Recusado,

        Emexecução,

        ControledeQualidade,

        AprovadoQualidade,

        ReprovadoQualidade,   

        EsperandoColeta,

        Unknown
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
        private const int BUSINESS_DAYS_LIMIT = 10;
        private const int WAITING_DAYS_LIMIT = 20;
        private const int NOT_APPROVED_FLAG = -999;

        [JsonPropertyName("_id")]
        public string Id {  get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("dateCreated")]
        [JsonConverter(typeof(DateConverter))]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }

        [JsonPropertyName("personName")]
        public string? PersonName { get; set; }

        [JsonPropertyName("personRazao")]
        public string? PersonRazao { get; set; }

        [JsonConverter(typeof(CustomEnumConverter<SaleType>))]
        public SaleType Type { get; set; } = SaleType.Unknown;

        public string DisplayType { get; set; }

        [JsonConverter(typeof(CustomEnumConverter<Status>))]
        public Status Status { get; set; }

        public string _displayStatus {  get; set; }
        public string DisplayStatus
        {
            get => _displayStatus;
            set
            {
                _displayStatus = value?.Trim() ?? string.Empty;

                if (Enum.TryParse(_displayStatus, true, out Status parsedStatus))
                {
                    Status = parsedStatus;
                }
            }
        }

        public bool IsStatusEditable => new HashSet<string>
        {
            "Aprovado",
            "Em Execução",
            "Controle de Qualidade",
            "Aprovado na Qualidade",
            "Reprovado na Qualidade",
            "Esperando Coleta"
        }.Contains(Status.ToString());

        public static bool IsLocalStatus(string status)
        {
            var localStatuses = new HashSet<string>
            {
                "Em Execução",
                "Controle de Qualidade",
                "Aprovado na Qualidade",
                "Reprovado na Qualidade",
                "Esperando Coleta"
            };

            return localStatuses.Contains(status);
        }

        public List<string> StatusList { get; } = GetStatusToComboBox.GetStatusToComboBoxList<Status>().GetRange(7, 4);

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; } = new();

        private List<CustomTag> _mappedTags;
        public List<CustomTag> MappedTags
        {
            get
            {
                if (_mappedTags == null)
                {
                    var tagMapper = new Tags();
                    _mappedTags = Tags
                        .Select(tag => tagMapper.GetCustomTagById(tag.TagId))
                        .Where(mappedTags => mappedTags != null)
                        .ToList();
                }

                return _mappedTags;
            }
        }

        [JsonPropertyName("approved")]
        public bool Approved { get; set; }

        public string? Observation {  get; set; }

        public bool IsManual { get; set; }

        public int Priority { get; set; }

        public bool IsPaused { get; set; }

        public bool IsStatusModified { get; set; } = false;

        public DateTime? ApprovedAt { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? ExpirationDate
        {
            get
            {
                if (ApprovedAt.HasValue || Status == Status.Aprovado) return null;

                return BusinessDayCalculator.CalculateExpirationDate(DateCreated);
            }
        }

        public int? RemainingBusinessDays
        {
            get
            {
                if (!ApprovedAt.HasValue) return NOT_APPROVED_FLAG;

                DateTime finalDate = ApprovedAt.Value.AddDays(BUSINESS_DAYS_LIMIT);
                return BusinessDayCalculator.CalculateBusinessDays(DateTime.Now, finalDate);
            }
        }
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
