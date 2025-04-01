using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using NeuroApp.ViewModels;

namespace NeuroApp
{
    public partial class SupportGuideDialog : UserControl
    {
        public SupportGuideDialog(string equipment)
        {
            InitializeComponent();
            DataContext = new SupportGuideViewModel(equipment);
        }
    }
} 