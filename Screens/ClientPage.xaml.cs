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

namespace NeuroApp.Screens
{
    public partial class ClientPage : Page
    {
        public ClientPage(string clientId)
        {
            InitializeComponent();
            LoadClientInfo(clientId);
        }

        private void LoadClientInfo(string Id)
        {
            Classes.Person person = new();
            
            string name = person.name;
            string nature = person.Nature;
            string CpfOrCnpj = person.CpfOrCnpj;
            string type = person.Type.ToString();
            string icmsCont = person.IcmsCont.ToString();
            string phone = person.phone;
            string cellphone = person.cellphone;
            string mainContact = person.mainContact;
            string email = person.email;
            string endereco = $"{person.address.street}, {person.address.number}";
            string cep = person.address.cep;
            string neighborhood = person.address.neighborhood;
            string City_Uf = $"{person.address.city}/{person.address.uf}";
            string country = person.address.country;
            
        }
    }
}
