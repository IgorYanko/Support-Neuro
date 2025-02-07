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
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("razao")]
        public string? Razao { get; set; }

        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        [JsonPropertyName("cpf")]
        public string? Cpf { get; set; }

        public string CpfOrCnpj
        {
            get
            {
                if (!string.IsNullOrEmpty(Cnpj) && long.TryParse(Cnpj, out _))
                {
                    return Convert.ToUInt64(Cnpj).ToString(@"00\.000\.000/0000-00");
                }
                else if (!string.IsNullOrEmpty(Cpf) && long.TryParse(Cpf, out _))
                {
                    return Convert.ToUInt64(Cpf).ToString(@"000\.000\.000-00");
                }

                return string.Empty;
            }
        }

        [JsonPropertyName("rg")]
        public string? Rg { get; set; }

        [JsonPropertyName("inscricao")]
        public string Inscricao { get; set; }

        [JsonPropertyName("inscricaoMunicipal")]
        public string InscricaoMunicipal { get; set; }

        [JsonPropertyName("icmsCont")]
        public IcmsCont IcmsCont { get; set; }

        [JsonPropertyName("type")]
        public List<PersonType> Type { get; set; } = new();

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("nature")]
        public Nature Nature { get; set; }

        [JsonPropertyName("address")]
        public PersonAddress Address { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("cellphone")]
        public string? Cellphone { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }

        [JsonPropertyName("integrations")]
        public List<PersonIntegration>? Integrations { get; set; }

        [JsonPropertyName("mainContact")]
        public string MainContact { get; set; }
    }


    public class PersonAddress
    {
        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("uf")]
        public string Uf { get; set; }

        [JsonPropertyName("cep")]
        public string Cep { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }
    }

    public class PersonIntegration
    {
        public string Name { get; set; }
        public string Id { get; set; }
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