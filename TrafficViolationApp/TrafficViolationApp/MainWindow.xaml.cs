using System.Windows;
using TrafficViolationApp.ViewModels;
using TrafficViolationApp.Views;

namespace TrafficViolationApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
            mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(mainViewModel) });
        }
    }
}