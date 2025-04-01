using System.Collections.ObjectModel;
using NeuroApp.Api;

namespace NeuroApp.Classes
{
    public class SupportFlowNode
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string YesNextNodeId { get; set; }
        public string NoNextNodeId { get; set; }
        public ObservableCollection<string> Solutions { get; set; }
        public bool IsFinal { get; set; }
        public string Category { get; set; }

        public SupportFlowNode(string id, string question, string category, string yesNextNodeId = null, string noNextNodeId = null)
        {
            Id = id;
            Question = question;
            Category = category;
            YesNextNodeId = yesNextNodeId;
            NoNextNodeId = noNextNodeId;
            Solutions = new ObservableCollection<string>();
            IsFinal = false;
        }

        public void AddSolution(string solution)
        {
            Solutions.Add(solution);
        }

        public void AddSolutions(IEnumerable<string> solutions)
        {
            foreach (var solution in solutions)
            {
                Solutions.Add(solution);
            }
        }
    }

    public class SupportFlowManager
    {
        public Dictionary<string, Dictionary<string, SupportFlowNode>> FlowGuides { get; }

        public SupportFlowManager()
        {
            FlowGuides = new Dictionary<string, Dictionary<string, SupportFlowNode>>();
            InitializeFlows();
        }

        private void InitializeFlows()
        {
            InitializeVideoFrenzelFlow();
            InitializeSivecPlusFlow();
            InitializeVecwinFlow();
            InitializeOtoFlow();
        }

        private void InitializeVideoFrenzelFlow()
        {
            var flow = new Dictionary<string, SupportFlowNode>();
            
            var startNode = new SupportFlowNode("start", "A imagem está aparecendo na tela?", "Diagnóstico Inicial", "image_fading", "check_connection");
            var imageFading = new SupportFlowNode("image_fading", "Ela some ao movimentar a câmera ou de forma repentina?", "Instabilidade na Imagem", "test_other_usb", "check_focus");
            var checkFocus = new SupportFlowNode("check_focus", "A imagem está nítida?", "Qualidade de Imagem", "working_fine", "focus_issue");
            var checkConnection = new SupportFlowNode("check_connection", "O computador reconhece a câmera no gerenciador de dispositivos?", "Conectividade", "check_drivers", "test_other_usb");
            
            var checkDrivers = new SupportFlowNode("check_drivers", "Os drivers estão atualizados?", "Drivers", "test_other_usb", "update_drivers");
            var testOtherUsb = new SupportFlowNode("test_other_usb", "Testou em outra porta USB?", "Conectividade", "test_other_pc", "try_other_usb");
            var testOtherPc = new SupportFlowNode("test_other_pc", "Testou em outro computador e o problema persiste?", "Hardware", "send_support", "pc_issue");

            var workingFine = new SupportFlowNode("working_fine", "Sistema funcionando corretamente!", "Conclusão")
            {
                IsFinal = true
            };
            workingFine.AddSolution("O sistema está funcionando normalmente.");

            var focusIssue = new SupportFlowNode("focus_issue", "Problema de foco identificado:", "Solução")
            {
                IsFinal = true
            };
            focusIssue.AddSolutions(new[]
            {
                "1. Ajuste o foco da lente girando o anel de foco",
                "2. Verifique se há sujeira na lente",
                "3. Limpe a lente com um pano macio se necessário"
            });

            var updateDrivers = new SupportFlowNode("update_drivers", "Atualização de drivers necessária:", "Solução")
            {
                IsFinal = true
            };
            updateDrivers.AddSolutions(new[]
            {
                "1. Procure os drivers na internet ou pelo próprio gerenciador de dispositivos",
                "2. Baixe os drivers mais recentes",
                "3. Instale os drivers",
                "4. Reinicie o computador"
            });

            var tryOtherUsb = new SupportFlowNode("try_other_usb", "Teste em outra porta USB:", "Solução")
            {
                IsFinal = true
            };
            tryOtherUsb.AddSolutions(new[]
            {
                "1. Desconecte o dispositivo",
                "2. Conecte em outra porta USB",
                "3. Aguarde o Windows reconhecer o dispositivo",
                "4. Se não funcionar, teste em outro computador"
            });

            var pcIssue = new SupportFlowNode("pc_issue", "Problema no computador:", "Solução")
            {
                IsFinal = true
            };
            pcIssue.AddSolutions(new[]
            {
                "1. Verifique se há outros dispositivos USB com problema",
                "2. Atualize os drivers USB do computador",
                "3. Se persistir, pode ser necessário formatar o computador"
            });

            var sendSupport = new SupportFlowNode("send_support", "Enviar para assistência técnica:", "Solução")
            {
                IsFinal = true
            };
            sendSupport.AddSolutions(new[]
            {
                "1. Entre em contato com o suporte",
                "2. Relate os testes realizados",
                "3. Forneça instruções para envio do equipamento ao cliente"
            });

            var nodes = new[] { startNode, imageFading, checkFocus, checkConnection, checkDrivers, testOtherUsb, 
                              testOtherPc, workingFine, focusIssue, updateDrivers, tryOtherUsb, 
                              pcIssue, sendSupport };
            
            foreach (var node in nodes)
            {
                flow[node.Id] = node;
            }

            FlowGuides["Vídeo Frenzel"] = flow;
        }

        private void InitializeSivecPlusFlow()
        {
            var flow = new Dictionary<string, SupportFlowNode>();

            var startNode = new SupportFlowNode("start", "O exame está dando interferência?", "Diagnóstico Inicial", "check_electrodes", "check_connection");
            var checkElectrodes = new SupportFlowNode("check_electrodes", "Os eletrodos não foram limpos ou estão danificados?", "Conectividade", "electrodes_issue", "check_regular_calibration");
            var checkRegularCalibration = new SupportFlowNode("check_regular_calibration", "O equipamento está com a calibração em dia?(Menos de 12 meses)", "Calibração", "working_fine", "send_to_calibrate");
            var checkConnection = new SupportFlowNode("check_connection", "O computador está reconhecendo o Sivec?", "Conectividade", "check_drivers", "test_other_usb");

            var checkDrivers = new SupportFlowNode("check_drivers", "Os drivers estão atualizados?", "Drivers", "test_other_usb", "update_drivers");
            var testOtherUsb = new SupportFlowNode("test_other_usb", "Testou em outra porta USB?", "Conectividade", "test_other_pc", "try_other_usb");
            var testOtherPc = new SupportFlowNode("test_other_pc", "Testou em outro computador e o problema persiste?", "Hardware", "send_support", "pc_issue");

            var electrodesIssue = new SupportFlowNode("electrodes_issue", "Compre novos eletrodos:", "Solução")
            {
                IsFinal = true
            };
            electrodesIssue.AddSolutions(new[]
            {
                "1. Retire o excesso de pasta condutora dos eletrodos com um pano ou papel",
                "2. Utilize uma escova de dente com água e sabão neutro para esfregar o eletrodo e depois deixe secar",
                "3. Caso estejam danificados, adquira um novo conjunto de eletrodos"
            });

            var sendToCalibrate = new SupportFlowNode("send_to_calibrate", "Enviar para assistência técnica:", "Solução")
            {
                IsFinal = true
            };
            sendToCalibrate.AddSolutions(new[]
            {
                "1. Forneça instruções para envio do equipamento ao cliente",
                "2. O orçamento será enviado para a aprovação antes da calibração"
            });

            var updateDrivers = new SupportFlowNode("update_drivers", "Atualização de drivers necessária:", "Solução")
            {
                IsFinal = true
            };
            updateDrivers.AddSolutions(new[]
            {
                "1. Procure os drivers na internet ou pelo próprio gerenciador de dispositivos",
                "2. Baixe os drivers mais recentes",
                "3. Instale os drivers",
                "4. Reinicie o computador"
            });

            var tryOtherUsb = new SupportFlowNode("try_other_usb", "Teste em outra porta USB:", "Solução")
            {
                IsFinal = true
            };
            tryOtherUsb.AddSolutions(new[]
            {
                "1. Desconecte o dispositivo",
                "2. Conecte em outra porta USB",
                "3. Aguarde o Windows reconhecer o dispositivo",
                "4. Se não funcionar, teste em outro computador"
            });

            var pcIssue = new SupportFlowNode("pc_issue", "Problema no computador:", "Solução")
            {
                IsFinal = true
            };
            pcIssue.AddSolutions(new[]
            {
                "1. Verifique se há outros dispositivos USB com problema",
                "2. Atualize os drivers USB do computador",
                "3. Se persistir, pode ser necessário formatar o computador"
            });

            var sendSupport = new SupportFlowNode("send_support", "Enviar para assistência técnica:", "Solução")
            {
                IsFinal = true
            };
            sendSupport.AddSolutions(new[]
            {
                "1. Entre em contato com o suporte",
                "2. Relate os testes realizados",
                "3. Forneça instruções para envio do equipamento ao cliente"
            });

            var workingFine = new SupportFlowNode("working_fine", "Sistema funcionando corretamente!", "Conclusão")
            {
                IsFinal = true
            };
            workingFine.AddSolution("O sistema está funcionando normalmente.");

            var nodes = new[] { startNode, checkElectrodes, checkRegularCalibration, checkConnection, checkDrivers, testOtherUsb, testOtherPc, electrodesIssue, sendToCalibrate, updateDrivers, tryOtherUsb, pcIssue, sendSupport, workingFine };

            foreach (var node in nodes)
            {
                flow[node.Id] = node;
            }

            FlowGuides["Sivec Plus"] = flow;
        }

        private void InitializeVecwinFlow()
        {
            var flow = new Dictionary<string, SupportFlowNode>();

            var startNode = new SupportFlowNode("start", "Há interferência no exame?", "Diagnóstico Inicial", "oto_interference", "smoke_presence");
            var smokePresence = new SupportFlowNode("smoke_presence", "Há fumaça ou cheiro de queimado vindo do equipamento?", "Cheiro de Queimado ou Fumaça", "send_support", "other_problems");
            var otoInterference = new SupportFlowNode("oto_interference", "Há interferência no exame ao ligar o oto?", "Interferência ao ligar o oto", "electric_problem", "send_photo");

            var workingFine = new SupportFlowNode("working_fine", "Sistema funcionando corretamente!", "Conclusão")
            {
                IsFinal = true
            };
            workingFine.AddSolution("O sistema está funcionando normalmente.");

            var sendSupport = new SupportFlowNode("send_support", "Enviar para assistência técnica:", "Solução")
            {
                IsFinal = true
            };
            sendSupport.AddSolutions(new[]
            {
                "1. Entre em contato com o suporte",
                "2. Relate os testes realizados",
                "3. Forneça instruções para envio do equipamento ao cliente"
            });

            var eletricProblem = new SupportFlowNode("electric_problem", "Problema na parte elétrica:", "Problema elétrico")
            {
                IsFinal = true
            };
            eletricProblem.AddSolutions(new[]
            {
                "1. Teste em outra tomada, aterrada e de mesma voltagem",
                "2. Caso o problema deixe de aparecer, o problema deve estar no aterramento da tomada",
                "3. Se o problema persistir, teste se possível o aparelho em outro lugar",
                "4. Caso o problema deixe de aparecer, o problema deve ser o aterramento do lugar",
                "5. Se nada resolver, forneça as instruções para o envio do aparelho"
            });

            var sendPhoto = new SupportFlowNode("send_photo", "Enviar foto do sinal", "Envio de fotos")
            { 
                IsFinal = true
            };
            sendPhoto.AddSolutions(new[]
            {
                "1. Peça a foto da colocação de eletrodos",
                "2. Peça a foto dos sinais gravados",
                "3. Encaminhe os arquivos para o suporte"
            });


            var otherProblems = new SupportFlowNode("other_problems", "Possíveis outros problemas:", "Outros problemas")
            {
                IsFinal = true
            };
            otherProblems.AddSolutions(new[]
            {    
                "1. Caso o problema não esteja descrito, contatar suporte",
                "2. Relate os testes realizados",
                "3. Forneça instruções para envio do equipamento ao cliente"
            });

            var nodes = new[] { startNode, smokePresence, workingFine, sendSupport, otherProblems, sendPhoto, otoInterference, eletricProblem};

            foreach (var node in nodes)
            {
                flow[node.Id] = node;
            }

            FlowGuides["Vecwin"] = flow;
        }

        private void InitializeOtoFlow()
        {
            var flow = new Dictionary<string, SupportFlowNode>();

            var startNode = new SupportFlowNode("start", "Há fumaça ou cheiro de queimado vindo do equipamento?", "Cheiro de Queimado ou Fumaça", "send_support", "check_temperature");
            var checkTemperature = new SupportFlowNode("check_temperature", "O oto está esquentando/esfriando normalmente?", "Problema Peltier", "check_irrigator", "send_support");
            var checkIrrigator = new SupportFlowNode("check_irrigator", "O ar está saindo do irrigador?", "Problema no irrigador", "check_fan", "check_connector");
            var checkFan = new SupportFlowNode("check_fan", "A ventoinha está funcionando?", "Problema na Ventoinha", "working_fine", "fan_problem");
            var checkConnector = new SupportFlowNode("check_connector", "Está saindo ar do conector traseiro?", "Problema no conector", "send_support", "send_support");

            var fanProblem = new SupportFlowNode("fan_problem", "Problema na ventoinha:", "Solução")
            {
                IsFinal = true,
            };
            fanProblem.AddSolutions(new[]
            {
                "1. Evite ligar o oto para evitar a queima do peltier",
                "2. Forneça instruções para envio do equipamento ao cliente",
                "3.Sugerir a coleta reversa para o cliente (frete por conta do cliente)"
            });

            var sendSupport = new SupportFlowNode("send_support", "Enviar para assistência técnica:", "Solução")
            {
                IsFinal = true
            };
            sendSupport.AddSolutions(new[]
            {
                "1. Entre em contato com o suporte",
                "2. Relate os testes realizados",
                "3. Forneça instruções para envio do equipamento ao cliente"
            });

            var workingFine = new SupportFlowNode("working_fine", "Sistema funcionando corretamente!", "Conclusão")
            {
                IsFinal = true
            };
            workingFine.AddSolution("O sistema está funcionando normalmente.");

            var nodes = new[] { startNode, checkTemperature, checkIrrigator, checkFan, checkConnector, fanProblem, sendSupport, workingFine };

            foreach (var node in nodes)
            {
                flow[node.Id] = node;
            }

            FlowGuides["Otocalorímetro"] = flow;
        }

        public SupportFlowNode GetNode(string equipment, string nodeId)
        {
            if (FlowGuides.TryGetValue(equipment, out var flow))
            {
                if (flow.TryGetValue(nodeId, out var node))
                {
                    return node;
                }
            }
            return null;
        }

        public SupportFlowNode GetStartNode(string equipment)
        {
            return GetNode(equipment, "start");
        }

        public SupportFlowNode GetNextNode(string equipment, string currentNodeId, bool isYesResponse)
        {
            var currentNode = GetNode(equipment, currentNodeId);
            if (currentNode == null) return null;

            var nextNodeId = isYesResponse ? currentNode.YesNextNodeId : currentNode.NoNextNodeId;
            return GetNode(equipment, nextNodeId);
        }
    }
}