using CommunityToolkit.Mvvm.ComponentModel;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private object currentView;
        public Action OnLogout { get; set; }
        public MainViewModel()
        {
            CurrentView = new LoginView { DataContext = new LoginViewModel(this) };
        }

        public void NavigateTo(object view)
        {
            CurrentView = view;
        }
    }
}