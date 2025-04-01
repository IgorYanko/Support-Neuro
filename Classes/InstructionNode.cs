using System.Collections.ObjectModel;

namespace NeuroApp.Classes
{
    public class InstructionNode
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public ObservableCollection<InstructionNode> Instructions { get; set; }

        public bool HasChildren => Instructions.Count > 0;

        public InstructionNode(string title, string content = null)
        {
            Title = title;
            Content = content;
            Instructions = new();
        }
    }
}
