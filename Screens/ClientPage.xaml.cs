using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeuroApp.Api;
using NeuroApp.Classes;

namespace NeuroApp.Screens
{
    public partial class ClientPage : Page
    {
        public string Name { get; set; }
        public string Nature { get; set; }
        public string CpfOrCnpj { get; set; }
        public string Type { get; set; }
        public string IcmsCont { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public string MainContact { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Cep { get; set; }
        public string Neighborhood { get; set; }
        public string CityUf { get; set; }
        public string Country { get; set; }

        public ClientPage(Person client)
        {
            InitializeComponent();
            LoadClientInfo(client);
            this.DataContext = this;
        }

        private void LoadClientInfo(Person client)
        {
            Name = client.name ?? "----";
            Nature = client.Nature ?? "----";
            CpfOrCnpj = client.CpfOrCnpj ?? "----";
            Type = client.Type.ToString() ?? "----";
            IcmsCont = client.IcmsCont.ToString() ?? "----";
            Phone = client.phone ?? "----";
            Cellphone = client.cellphone ?? "----";
            MainContact = client.mainContact ?? "----";
            Email = client.email ?? "----";

            if (client.address != null)
                Address = $"{client.address.street}, {client.address.number}";
            else
                Address = "----";
            
            Cep = !string.IsNullOrEmpty(client.address.cep) && long.TryParse(client.address.cep, out var cep) 
                ? string.Format("{0:#####-###}", cep)
                : "----";

            Neighborhood = client.address.neighborhood ?? "----";
            
            if (client.address.city != null && client.address.uf != null)
                CityUf = $"{client.address.city}/{client.address.uf}";
            else
                CityUf = "----";

            Country = client.address.country ?? "----";
        }
    }
}
