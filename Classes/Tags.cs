using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class CustomTag
    {
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Name { get; set; }

        public CustomTag(string color, string textColor, string name)
        {
            Color = color;
            TextColor = textColor;
            Name = name;
        }
    }

    public class Tags
    {
        private readonly Dictionary<string, CustomTag> tagDictionary;

        public Tags()
        {
            tagDictionary = new Dictionary<string, CustomTag>
            {
                { "67f802fb3145c3c1a6fc1d97", new CustomTag("#FC38FF", "#FFFFFF", "Aguardando Cliente") },
                { "67ea9dc6a6b4f2600fe6e93d", new CustomTag("#FC38FF", "#FFFFFF", "Aguardando Cliente") },

                { "67f42582dede58ad40c7ab5e", new CustomTag("#893939", "#FFFFFF", "Aguardando matéria-prima") }, 
                { "67a9e528b4b5e6dff68ec07e", new CustomTag("#893939", "#FFFFFF", "Aguardando matéria-prima") },

                { "641d921fbd8638ee915d7c31", new CustomTag("#FF6600", "#FFFFFF", "ATENÇÃO") },
                { "67dd6c412d309a48df8baff3", new CustomTag("#FF6600", "#FFFFFF", "ATENÇÃO") },

                { "641d91bebd8638ee915d5155", new CustomTag("#C30F0F", "#FFFFFF", "URGENTE") },
                { "67dd6c4db5c24485b0eb0242", new CustomTag("#C30F0F", "#FFFFFF", "URGENTE") },
                
                { "66f56c28dbe9ae1bca04da01", new CustomTag("#DDBF27", "#353535", "Orçamento Realizado") },
                { "67dd7065a853d4325710c96e", new CustomTag("#FFD500", "#353535", "Orçamento Realizado") },

                { "641d91fdbd8638ee915d68f7", new CustomTag("#D8BFD8", "#353535", "Em execução") },
                { "65538e7da2f772013a7f4a69", new CustomTag("#D8BFD8", "#353535", "Em execução") },
                
                { "6570611b767fc694ef30eae1", new CustomTag("#DC143C", "#FFFFFF", "Garantia de Serviços Técnicos") },
                { "67fe543658f5f7026e320ced", new CustomTag("#DC143C", "#FFFFFF", "Garantia de Serviços Técnicos") },

                { "682790ee954a2e7154ee7a5f", new CustomTag("#DE3163", "#FFFFFF", "Garantia de Vendas") },
                { "67fe5f0774b58112b7458a85", new CustomTag("#DE3163", "#FFFFFF", "Garantia de Vendas") },

                { "66f56c659d3ccc33809c7ac1", new CustomTag("#00FF00", "#FFFFFF", "Orçamento Aprovado") },
                { "66f5922c647f50302398bd9d", new CustomTag("#00FF00", "#353535", "Orçamento Aprovado") },
                  
                { "66f438f5bae5d3cdfc5c029e", new CustomTag("#000000", "#FFFFFF", "Cortesia/Parceria") },

                { "67f7fa8c76fdec8ffadce658", new CustomTag("#080707", "#FFFFFF", "Upgrade OTO") },
                { "67f801b9db8957c2007e624f", new CustomTag("#080707", "#FFFFFF", "Upgrade OTO") },

                { "66f16109f30fb9c33bbba4cd", new CustomTag("#0040FF", "#FFFFFF", "Concluído") },
                { "67f7fad8211f7ebefe6e8159", new CustomTag("#0040FF", "#FFFFFF", "Concluído") },

                { "67a604e090f61408b2242f8d", new CustomTag("#708090", "#FFFFFF", "Orçamento Reprovado") },

                { "66f1621642e2594cd72206aa", new CustomTag("#A84CE6", "#FFFFFF", "Realizar") },

                { "65b25af2147b88b8768f10f6", new CustomTag("#F25D0D", "#FFFFFF", "Suporte Operacional") },

                { "65b25a77147b88b8768ebb19", new CustomTag("#7BA05B", "#FFFFFF", "Instalação de Software") },

                { "67b4ab82b887c7571496c629", new CustomTag("#5F9EA0", "#FFFFFF", "Atualização de software") }

            };
        }

        public CustomTag? GetCustomTagById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return tagDictionary.TryGetValue(id.Trim(), out var tag)
                ? tag
                : null;
        }
    }
}
