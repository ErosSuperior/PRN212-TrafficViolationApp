using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class ReportViewModel : ObservableObject, IDisposable
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
        private bool _disposed = false;

        public ReportViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void SubmitReport()
        {
            try
            {
                // Kiểm tra biển số xe có tồn tại trong cơ sở dữ liệu không
                var vehicle = _context.Vehicles?.FirstOrDefault(v => v.PlateNumber == PlateNumber);
                if (vehicle == null)
                {
                    MessageBox.Show("Biển số xe không tồn tại trong hệ thống. Vui lòng kiểm tra lại!");
                    return;
                }

                var report = new Report
                {
                    ReporterId = _currentUserId,
                    ViolationType = ViolationType,
                    Description = Description,
                    PlateNumber = PlateNumber,
                    Location = Location,
                    ImageUrl = string.IsNullOrWhiteSpace(ImageURL) ? null : ImageURL,
                    VideoUrl = string.IsNullOrWhiteSpace(VideoURL) ? null : VideoURL,
                    ReportDate = DateTime.Now,
                    Status = "Pending"
                };
                _context.Reports?.Add(report);
                _context.SaveChanges();
                
                MessageBox.Show("Phản ánh đã được gửi!");
                
                // Clear form after submission
                ViolationType = Description = PlateNumber = Location = ImageURL = VideoURL = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi báo cáo: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ViewNotifications()
        {
            try
            {
                _mainViewModel.NavigateTo(new NotificationView { DataContext = new NotificationViewModel(_currentUserId, _mainViewModel) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển trang: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ViewViolations()
        {
            try
            {
                _mainViewModel.NavigateTo(new ViolationView { DataContext = new ViolationViewModel(_currentUserId, _mainViewModel) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển trang: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void ViewMyVehicles()
        {
            try
            {
                _mainViewModel.NavigateTo(new MyVehiclesView { DataContext = new MyVehiclesViewModel(_currentUserId, _mainViewModel) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển trang: {ex.Message}");
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                
                _disposed = true;
            }
        }
    }
}