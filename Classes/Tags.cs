using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class CustomTag
    {
        public string Id {  get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Name { get; set; }

        public CustomTag(string id, string color, string textColor, string name) 
        { 
            Id = id;
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
                { "641d91bebd8638ee915d5155", new CustomTag("641d91bebd8638ee915d5155", "#C20F0F", "#FFFFFF", "URGENTE") }, //URGENTE
                { "641d91dfbd8638ee915d5506", new CustomTag("641d91dfbd8638ee915d5506", "#923535", "#FFFFFF", "IMPORTANTE") }, //IMPORTANTE
                { "641d91fdbd8638ee915d68f7", new CustomTag("641d91fdbd8638ee915d68f7", "#CCF2F5", "#353535", "Em execução") }, //Em Execução - Orçamento
                { "641d921fbd8638ee915d7c31", new CustomTag("641d921fbd8638ee915d7c31", "#D49711", "#FFFFFF", "ATENÇÃO") }, //ATENÇÃO
                { "641d9254bd8638ee915d980d", new CustomTag("641d9254bd8638ee915d980d", "#BE11D4", "#FFFFFF", "ACELERAR") }, //ACELERAR
                { "6570611b767fc694ef30eae1", new CustomTag("6570611b767fc694ef30eae1", "#F0ADAD", "#353535", "Garantia de Serviços Técnicos") }, //Garantia de Serviços Técnicos
                { "65b25a77147b88b8768ebb19", new CustomTag("65b25a77147b88b8768ebb19", "#2DD758", "#FFFFFF", "Instalação Sivec Plus") }, //Instalação Sivec Plus
                { "65b25aad147b88b8768eecd4", new CustomTag("65b25aad147b88b8768eecd4", "#87B592", "#353535", "Instalação Vídeo Frenzel") }, //Instalação Vídeo Frenzel
                { "65b25af2147b88b8768f10f6", new CustomTag("65b25af2147b88b8768f10f6", "#F25D0D", "#FFFFFF", "Suporte Operacional") }, //Suporte Operacional
                { "66f16109f30fb9c33bbba4cd", new CustomTag("66f16109f30fb9c33bbba4cd", "#26EF0B", "#FFFFFF", "Realizado") }, //Realizado
                { "66f1621642e2594cd72206aa", new CustomTag("66f1621642e2594cd72206aa", "#A84CE6", "#FFFFFF", "Realizar") }, //Realizar
                { "66f56c28dbe9ae1bca04da01", new CustomTag("66f56c28dbe9ae1bca04da01", "#DDBF27", "#353535", "Orçamento Realizado") }, //Orçamento Realizado
                { "66f56c659d3ccc33809c7ac1", new CustomTag("66f56c659d3ccc33809c7ac1", "#1DC320", "#FFFFFF", "Orçamento Aprovado") },  //Orçamento Aprovado
                  
                { "65538e7da2f772013a7f4a69", new CustomTag("65538e7da2f772013a7f4a69", "#2464CC", "#FFFFFF", "Em execução") }, //Em Execução - Ordem de Serviço
                { "66f438f5bae5d3cdfc5c029e", new CustomTag("66f438f5bae5d3cdfc5c029e", "#F90B0", "#FFFFFF", "Cortesia/Parceria") }, //Cortesia/Parceria
                { "66f5922c647f50302398bd9d", new CustomTag("66f5922c647f50302398bd9d", "#F6AB09", "#353535", "Orçamento Aprovado") }, //Orçamento Aprovado - Ordem de Serviço
                { "675055a2c217396456ade083", new CustomTag("675055a2c217396456ade083", "#094EF1", "#FFFFFF", "Em Andamento") }, //Em Andamento
                { "66f6a6d16197be8762af078f", new CustomTag("66f6a6d16197be8762af078f", "#F9A615", "#353535", "Orçamento Finalizado") }, //Orçamento Finalizado
                { "6798d4c54f7bc4c825d878f7", new CustomTag("6798d4c54f7bc4c825d878f7", "#19E6E2", "#353535", "Controle de Qualidade") }, //Controle de Qualidade
                { "67a604e090f61408b2242f8d", new CustomTag("67a604e090f61408b2242f8d", "#121111", "#FFFFFF", "Orçamento Reprovado") }, //Orçamento Reprovado
                { "67a9e528b4b5e6dff68ec07e", new CustomTag("67a9e528b4b5e6dff68ec07e", "#4C2A2A", "#FFFFFF", "Aguardando matéria-prima") }, //Aguardando matéria-prima
                { "67dd7065a853d4325710c96e", new CustomTag("67dd7065a853d4325710c96e", "#EAA510", "#353535", "Orçamento realizado") },
                { "67f801b9db8957c2007e624f", new CustomTag("67f801b9db8957c2007e624f", "#080707", "#FFFFFF", "Upgrade OTO") }
                //{ "66f56c28dbe9ae1bca04da01", new CustomTag("66f56c28dbe9ae1bca04da01", "#DDBF27", "#353535", "Orçamento realizado") }, //Orçamento aprovado
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
