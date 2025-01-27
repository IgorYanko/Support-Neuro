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

namespace NeuroApp.Controls
{
    public partial class Backbar : UserControl
    {
        public static readonly DependencyProperty GoBackCommandProperty =
            DependencyProperty.Register(
                nameof(GoBackCommand),
                typeof(ICommand),
                typeof(Backbar),
                new PropertyMetadata(null));



        public ICommand GoBackCommand
        {
            get => (ICommand)GetValue(GoBackCommandProperty);
            set => SetValue(GoBackCommandProperty, value);
        }

        public Backbar()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (GoBackCommand != null && GoBackCommand.CanExecute(null))
            {
                GoBackCommand.Execute(null);
            }
        }

        private void GetPageName()
        {

        }

    }
}
