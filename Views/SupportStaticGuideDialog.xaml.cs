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
using NeuroApp.Classes;
using NeuroApp.ViewModels;

namespace NeuroApp.Views
{
    public partial class SupportStaticGuideDialog : UserControl
    {
        private SupportStaticGuideViewModel viewModel;

        public SupportStaticGuideDialog(string title)
        {
            InitializeComponent();
            viewModel = new(title);
            DataContext = viewModel;
        }
    }
}
