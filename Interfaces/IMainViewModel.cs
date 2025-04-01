using System.Windows.Input;
using NeuroApp.Classes;

namespace NeuroApp.Interfaces
{
    public interface IMainViewModel
    {
        ICommand GoBackCommand { get; }
        object CurrentView { get; set; }
        ICommand ShowCockpitScreen_ { get; }
        ICommand ShowCustomersScreen_ { get; }
        ICommand ShowSupportGuideCommand { get; }
        ICommand ShowWarrantyScreenCommand { get; }
        ResponsiveBigButtons ResponsiveBigButtons { get; set; }
        void ShowHomeScreen();
    }
} 