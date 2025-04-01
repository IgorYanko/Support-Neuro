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
        public string Name { get; set; } = string.Empty;
        public string Nature { get; set; } = string.Empty;
        public string CpfOrCnpj { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string IcmsCont { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string MainContact { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string CityUf { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public ClientPage(Person client)
        {
            InitializeComponent();
            LoadClientInfo(client);
            this.DataContext = this;
        }

        private void LoadClientInfo(Person client)
        {
            Name = client.Name ?? "----";
            Nature = client.Nature.ToString() ?? "----";
            CpfOrCnpj = client.CpfOrCnpj ?? "----";
            Type = client.Type.ToString() ?? "----";
            IcmsCont = client.IcmsCont.ToString() ?? "----";
            Phone = client.Phone ?? "----";
            Cellphone = client.Cellphone ?? "----";
            MainContact = client.MainContact ?? "----";
            Email = client.Email ?? "----";

            if (client.Address != null)
            {
                Address = $"{client.Address.Street}, {client.Address.Number}";
                Cep = !string.IsNullOrEmpty(client.Address.Cep) && long.TryParse(client.Address.Cep, out var cep) 
                    ? string.Format("{0:#####-###}", cep)
                    : "----";
                Neighborhood = client.Address.Neighborhood ?? "----";
                
                if (client.Address.City != null && client.Address.Uf != null)
                {
                    CityUf = $"{client.Address.City}/{client.Address.Uf}";
                }
                else
                {
                    CityUf = "----";
                }

                Country = client.Address.Country ?? "----";
            }
            else
            {
                Address = "----";
                Cep = "----";
                Neighborhood = "----";
                CityUf = "----";
                Country = "----";
            }
        }
    }
}
