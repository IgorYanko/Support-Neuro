using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class CustomTag
    {
        public string _Id {  get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }

        public CustomTag(string _id, string color, string textColor) 
        { 
            _Id = _id;
            Color = color;
            TextColor = textColor;
        }
    }

    public class Tags
    {
        private Dictionary<string, CustomTag> tagDictionary;

        public Tags()
        {
            tagDictionary = new Dictionary<string, CustomTag>
            {
                { "URGENTE", new CustomTag("641d91bebd8638ee915d5155", "#C20F0F", "#FFFFFF")},
                { "IMPORTANTE", new CustomTag("641d91dfbd8638ee915d5506", "#923535", "#FFFFFF")},
                { "Em Execução", new CustomTag("641d91fdbd8638ee915d68f7", "#CCF2F5", "#353535")},
                { "ATENÇÃO", new CustomTag("641d921fbd8638ee915d7c31", "#D49711", "#FFFFFF")},
                { "ACELERAR", new CustomTag("641d9254bd8638ee915d980d", "#BE11D4", "#FFFFFF")},
                { "Garantia de Serviços Técnicos", new CustomTag("6570611b767fc694ef30eae1", "#F0ADAD", "#353535")},
                { "Instalação Sivec Plus", new CustomTag("65b25a77147b88b8768ebb19", "#2DD758", "#FFFFFF")},
                { "Instalação Vídeo Frenzel", new CustomTag("65b25aad147b88b8768eecd4", "#87B592", "#353535")},
                { "Suporte Operacional", new CustomTag("65b25af2147b88b8768f10f6", "#F25D0D", "#FFFFFF")},
                { "Realizado", new CustomTag("66f16109f30fb9c33bbba4cd", "#26EF0B", "#FFFFFF")},
                { "Realizar", new CustomTag("66f1621642e2594cd72206aa", "#A84CE6", "#FFFFFF")},
                { "Orçamento realizado", new CustomTag("66f56c28dbe9ae1bca04da01", "#DDBF27", "#353535")},
                { "Orçamento Aprovado", new CustomTag("66f5922c647f50302398bd9d", "#F6AB09", "#353535")}
            };
        }

        public CustomTag GetCustomTagById(string id)
        {
            return tagDictionary.Values
                .FirstOrDefault(tag => tag._Id.Trim().Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
