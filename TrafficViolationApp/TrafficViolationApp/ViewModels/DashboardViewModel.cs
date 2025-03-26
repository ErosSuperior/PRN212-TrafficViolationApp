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
        }

        [RelayCommand]
        private void Approve()
        {
            if (SelectedReport != null && SelectedReport.Status == "Pending")
            {
                SelectedReport.Status = "Approved";
                SelectedReport.ProcessedBy = _currentUserId;
                var violation = new Violation
                {
                    ReportId = SelectedReport.ReportId,
                    PlateNumber = SelectedReport.PlateNumber,
                    ViolatorId = _context.Vehicles.First(v => v.PlateNumber == SelectedReport.PlateNumber).OwnerId,
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
    }
}