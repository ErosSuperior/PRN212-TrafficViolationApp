using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Report> reports = new();
        [ObservableProperty] private Report? selectedReport;
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private ObservableCollection<Report> filteredReports = new();

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;

        public DashboardViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            LoadReports();
        }

        private void LoadReports()
        {
            Reports = new ObservableCollection<Report>(_context.Reports.Where(r => r.Status == "Pending"));
            UpdateFilteredReports();
        }

        private void UpdateFilteredReports()
        {
            var filtered = Reports.Where(r => 
                r.PlateNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                r.ViolationType.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                r.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            );
            FilteredReports = new ObservableCollection<Report>(filtered);
        }

        partial void OnSearchTextChanged(string value)
        {
            UpdateFilteredReports();
        }

        [RelayCommand]
        private void Approve()
        {
            if (SelectedReport != null && SelectedReport.Status == "Pending")
            {
                // Kiểm tra xem xe có tồn tại không
                var vehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == SelectedReport.PlateNumber);
                if (vehicle == null)
                {
                    MessageBox.Show("Không tìm thấy xe với biển số này trong hệ thống!");
                    return;
                }

                SelectedReport.Status = "Approved";
                SelectedReport.ProcessedBy = _currentUserId;
                var violation = new Violation
                {
                    ReportId = SelectedReport.ReportId,
                    PlateNumber = SelectedReport.PlateNumber,
                    ViolatorId = vehicle.OwnerId,
                    FineAmount = 500000,
                    FineDate = DateTime.Now
                };
                _context.Violations.Add(violation);
                _context.Notifications.Add(new Notification
                {
                    UserId = SelectedReport.ReporterId,
                    Message = $"Phản ánh về {SelectedReport.PlateNumber} đã được phê duyệt.",
                    PlateNumber = SelectedReport.PlateNumber,
                    SentDate = DateTime.Now
                });
                _context.Notifications.Add(new Notification
                {
                    UserId = violation.ViolatorId.Value,
                    Message = $"Bạn bị phạt {violation.FineAmount} vì vi phạm {SelectedReport.ViolationType}.",
                    PlateNumber = SelectedReport.PlateNumber,
                    SentDate = DateTime.Now
                });
                _context.SaveChanges();
                LoadReports();
                MessageBox.Show("Đã phê duyệt!");
            }
        }

        [RelayCommand]
        private void Reject()
        {
            if (SelectedReport != null && SelectedReport.Status == "Pending")
            {
                SelectedReport.Status = "Rejected";
                SelectedReport.ProcessedBy = _currentUserId;
                _context.Notifications.Add(new Notification
                {
                    UserId = SelectedReport.ReporterId,
                    Message = $"Phản ánh về {SelectedReport.PlateNumber} đã bị từ chối.",
                    PlateNumber = SelectedReport.PlateNumber,
                    SentDate = DateTime.Now
                });
                _context.SaveChanges();
                LoadReports();
                MessageBox.Show("Đã từ chối!");
            }
        }

        [RelayCommand]
        private void ViewStats()
        {
            _mainViewModel.NavigateTo(new ReportStatsView { DataContext = new ReportStatsViewModel(_mainViewModel, _currentUserId) });
        }
        [RelayCommand]
        private void ViewNotifications()
        {
            _mainViewModel.NavigateTo(new NotificationView { DataContext = new NotificationViewModel(_currentUserId, _mainViewModel) });
        }
        [RelayCommand]
        private void AddVehicle()
        {
            _mainViewModel.NavigateTo(new AddVehicleView { DataContext = new AddVehicleViewModel(_currentUserId, _mainViewModel) });
        }

        [RelayCommand]
        private void ViewUsers()
        {
            _mainViewModel.NavigateTo(new UserListView { DataContext = new UserListViewModel(_currentUserId, _mainViewModel) });
        }
        [RelayCommand]
        private void ViewMyVehicles()
        {
            _mainViewModel.NavigateTo(new MyVehiclesView { DataContext = new MyVehiclesViewModel(_currentUserId, _mainViewModel) });
        }

        [RelayCommand]
        private void Logout()
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _mainViewModel.OnLogout?.Invoke();
                _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
            }
        }
    }
}