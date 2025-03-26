using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        [ObservableProperty] private string violationType = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string plateNumber = string.Empty;
        [ObservableProperty] private string location = string.Empty;
        [ObservableProperty] private string imageURL = string.Empty;
        [ObservableProperty] private string videoURL = string.Empty;

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;

        public ReportViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void SubmitReport()
        {
            var report = new Report
            {
                ReporterId = _currentUserId,
                ViolationType = ViolationType,
                Description = Description,
                PlateNumber = PlateNumber,
                Location = Location,
                ImageUrl = ImageURL,
                VideoUrl = VideoURL,
                ReportDate = DateTime.Now,
                Status = "Pending"
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            MessageBox.Show("Phản ánh đã được gửi!");
        }

        [RelayCommand]
        private void ViewNotifications()
        {
            _mainViewModel.NavigateTo(new NotificationView { DataContext = new NotificationViewModel(_currentUserId, _mainViewModel) });
        }

        [RelayCommand]
        private void ViewViolations()
        {
            _mainViewModel.NavigateTo(new ViolationView { DataContext = new ViolationViewModel(_currentUserId, _mainViewModel) });
        }
    }
}