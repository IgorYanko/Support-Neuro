namespace NeuroApp.Api
{
    public enum IcmsCont
    {
        ContribuinteDeICMS,
        ContribuinteIsento,
        NaoContribuinte
    }

    public enum Nature
    {
        Juridica,
        Fisica
    }

    public enum Type
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
        public string _id { get; set; }
        public string name { get; set; }
        public string razao { get; set; }
        public string? cnpj { get; set; }
        public string? cpf { get; set; }
        public string rg { get; set; }
        public string inscricao { get; set; }
        public string inscricaoMunicipal { get; set; }
        public IcmsCont? icmsCont { get; set; }
        public Type type { get; set; }
        public string code { get; set; }
        public Nature? nature { get; set; }
        public List<PersonAddress> address { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string cellphone { get; set; }
        public string details { get; set; }
        public List<PersonIntegration> integrations { get; set; }
    }

    public class PersonAddress
    {
        public string street { get; set; }
        public string number { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public string country { get; set; }
    }

    public class PersonIntegration
    {
        public string name { get; set; }
        public string id { get; set; }
    }

}
