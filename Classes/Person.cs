using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace NeuroApp.Classes
{
    public enum IcmsCont
    {
        ContribuinteDeICMS,
        ContribuinteIsento,
        NaoContribuinte
    }

    public enum Nature
    {
        Jurídica,
        Fisica
    }

    public enum PersonType
    {
        Cliente,
        Fornecedor,
        Funcionario,
        Vendedor,
        Transportador,
        Destinatario,
        Seguradora
    }

    public class Person
    {
        [JsonPropertyName("_id")]
        public string _id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("razao")]
        public string? razao { get; set; }

        [JsonPropertyName("cnpj")]
        public string? cnpj { get; set; }

        [JsonPropertyName("cpf")]
        public string? cpf { get; set; }

        public string CpfOrCnpj
        {
            get
            {
                if (!string.IsNullOrEmpty(cnpj) && cnpj != "---")
                {
                    return string.Format("{0:00\\.000\\.000/0000-00}", long.Parse(cnpj));
                }
                else if (!string.IsNullOrEmpty(cpf) && cnpj != "---")
                {
                    return string.Format("{0:000\\.000\\.000-00}", long.Parse(cpf));
                }

                return string.Empty;
            }
        }

        [JsonPropertyName("rg")]
        public string? rg { get; set; }

        [JsonPropertyName("inscricao")]
        public string inscricao { get; set; }

        [JsonPropertyName("inscricaoMunicipal")]
        public string inscricaoMunicipal { get; set; }

        [JsonPropertyName("icmsCont")]
        public string IcmsCont { get; set; }

        [JsonPropertyName("type")]
        public List<string> Type { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("nature")]
        public string Nature { get; set; }

        [JsonPropertyName("address")]
        public PersonAddress address { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("phone")]
        public string? phone { get; set; }

        [JsonPropertyName("cellphone")]
        public string? cellphone { get; set; }

        [JsonPropertyName("details")]
        public string details { get; set; }

        [JsonPropertyName("integrations")]
        public List<PersonIntegration>? integrations { get; set; }

        [JsonPropertyName("mainContact")]
        public string mainContact { get; set; }
    }


    public class PersonAddress
    {
        [JsonPropertyName("street")]
        public string street { get; set; }

        [JsonPropertyName("number")]
        public string number { get; set; }

        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("uf")]
        public string uf { get; set; }

        [JsonPropertyName("cep")]
        public string cep { get; set; }

        [JsonPropertyName("country")]
        public string country { get; set; }

        [JsonPropertyName("neighborhood")]
        public string neighborhood { get; set; }
    }

    public class PersonIntegration
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("response")]
        public List<Person> Response { get; set; }
    }

    public class ApiResponseSales
    {
        [JsonPropertyName("response")]
        public List<Sales> Response { get; set; }
    }

}