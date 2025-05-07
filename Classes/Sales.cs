using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace NeuroApp.Classes
{
    public static class SalesUtils
    {
        private static readonly HashSet<string> LocalStatuses = new()
        {
            "Em Execução",
            "Controle de Qualidade",
            "Aprovado na Qualidade",
            "Reprovado na Qualidade",
            "Esperando Coleta"
        };

        public static bool IsLocalStatus(string status) => LocalStatuses.Contains(status);
        
        public static bool IsStatusEditable(string status) => LocalStatuses.Contains(status) || status == "Aprovado";

        public static int? CalculateRemainingApprovedDays(DateTime? approvedAt)
        {
            if (!approvedAt.HasValue) return null;

            DateTime finalDate = BusinessDayCalculator.CalculateDeadline(approvedAt.Value);
            return DateTime.Now > finalDate ? -1 : BusinessDayCalculator.CalculateBusinessDays(DateTime.Now, finalDate);
        }

        public static int? CalculateRemainingDaysToCheckout(DateTime arrivalDate)
        {
            DateTime finalDate = BusinessDayCalculator.CalculateExpirationDate(arrivalDate);
            return DateTime.Now > finalDate ? -1 : BusinessDayCalculator.CalculateBusinessDays(DateTime.Now, finalDate);
        }

        public static int? CalculateRemainingQuotationDays(DateTime? quotationDate)
        {
            if (!quotationDate.HasValue) return null;

            DateTime finalDate = BusinessDayCalculator.CalculateQuotationExpirationDate(quotationDate.Value);
            return DateTime.Now > finalDate ? -1 : BusinessDayCalculator.CalculateBusinessDays(DateTime.Now, finalDate);
        }
    }

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

    public class Sales : INotifyPropertyChanged
    {
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

        public string _displayStatus;
        public string DisplayStatus
        {
            get => _displayStatus;
            set
            {
                if (_displayStatus != value)
                {
                    _displayStatus = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(DisplayStatus));
                }
            }
        }

        public List<string> StatusList { get; } = GetStatusToComboBox.GetStatusToComboBoxList<Status>().GetRange(7, 5);

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

        private string _observation;
        public string? Observation
        {
            get => _observation;
            set
            {
                _observation = value;
                OnPropertyChanged(nameof(Observation));
            }
        }

        public bool IsManual { get; set; }

        public int Priority { get; set; }

        public bool IsPaused { get; set; }

        public bool IsStatusModified { get; set; } = false;

        public bool Excluded {  get; set; }

        public DateTime? ApprovedAt { get; set; }
        
        public DateTime? Deadline { get; set; }

        public DateTime? CheckoutDate { get; set; }

        public DateTime? QuotationDate { get; set; }

        public DateTime? QuotationExpirationDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
