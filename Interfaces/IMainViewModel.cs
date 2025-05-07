using System.Collections.ObjectModel;
using System.Windows.Input;
using NeuroApp.Classes;

namespace NeuroApp.Interfaces
{
    public interface IMainViewModel
    {
        ICommand GoBackCommand { get; }
        ICommand ShowCockpitScreen { get; }
        ICommand ShowCustomersScreen { get; }
        ICommand ShowSupportGuideCommand { get; }
        ICommand ShowWarrantyScreenCommand { get; }
        ICommand ShowObservationsCommand { get; }

        object CurrentView { get; set; }
        ObservableCollection<Sales> Sales { get; }
        ResponsiveBigButtons ResponsiveBigButtons { get; set; }

        void ShowHomeScreen();
    }
} 