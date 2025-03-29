using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class AddVehicleViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] private string plateNumber = string.Empty;
        [ObservableProperty] private int ownerId;
        [ObservableProperty] private string brand = string.Empty;
        [ObservableProperty] private string model = string.Empty;
        [ObservableProperty] private int manufactureYear;
        [ObservableProperty] private ObservableCollection<Vehicle> vehicles = new();
        [ObservableProperty] private Vehicle? selectedVehicle; // Xe được chọn để xóa
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private ObservableCollection<Vehicle> filteredVehicles = new();

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;
        private bool _disposed = false;

        public AddVehicleViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            try
            {
                var allVehicles = _context.Vehicles?.ToList();
                Vehicles = new ObservableCollection<Vehicle>(allVehicles ?? new List<Vehicle>());
                FilteredVehicles = new ObservableCollection<Vehicle>(Vehicles); // Ban đầu hiển thị tất cả
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách xe: {ex.Message}");
                Vehicles = new ObservableCollection<Vehicle>();
                FilteredVehicles = new ObservableCollection<Vehicle>();
            }
        }

        [RelayCommand]
        private void SubmitVehicle()
        {
            try
            {
                if (string.IsNullOrEmpty(PlateNumber) || OwnerId <= 0 || string.IsNullOrEmpty(Brand) ||
                    string.IsNullOrEmpty(Model) || ManufactureYear <= 0)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                    return;
                }

                if (_context.Vehicles.Any(v => v.PlateNumber == PlateNumber))
                {
                    MessageBox.Show("Biển số xe đã tồn tại!");
                    return;
                }

                var newVehicle = new Vehicle
                {
                    PlateNumber = PlateNumber,
                    OwnerId = OwnerId,
                    Brand = Brand,
                    Model = Model,
                    ManufactureYear = ManufactureYear
                };

                _context.Vehicles.Add(newVehicle);

                _context.Notifications.Add(new Notification
                {
                    UserId = OwnerId,
                    Message = $"Xe {PlateNumber} đã được thêm vào hệ thống.",
                    PlateNumber = PlateNumber,
                    SentDate = DateTime.Now
                });

                int rowsAffected = _context.SaveChanges();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Thêm xe thành công!");
                    LoadVehicles(); // Cập nhật danh sách xe
                    PlateNumber = Brand = Model = string.Empty; // Xóa form sau khi thêm
                    OwnerId = ManufactureYear = 0;
                }
                else
                {
                    MessageBox.Show("Thêm xe thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm xe: {ex.Message}");
            }
        }

        [RelayCommand]
        private void DeleteVehicle()
        {
            try
            {
                if (SelectedVehicle == null)
                {
                    MessageBox.Show("Vui lòng chọn một xe để xóa!");
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn xóa xe {SelectedVehicle.PlateNumber}?", 
                    "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                // Kiểm tra xem xe có liên quan đến vi phạm hoặc báo cáo không
                if (_context.Reports?.Any(r => r.PlateNumber == SelectedVehicle.PlateNumber) == true ||
                    _context.Violations?.Any(v => v.PlateNumber == SelectedVehicle.PlateNumber) == true)
                {
                    MessageBox.Show("Không thể xóa xe này vì nó đã liên quan đến báo cáo hoặc vi phạm!");
                    return;
                }

                _context.Vehicles.Remove(SelectedVehicle);

                // Thêm thông báo cho chủ xe
                _context.Notifications?.Add(new Notification
                {
                    UserId = SelectedVehicle.OwnerId,
                    Message = $"Xe {SelectedVehicle.PlateNumber} đã bị xóa khỏi hệ thống.",
                    PlateNumber = SelectedVehicle.PlateNumber,
                    SentDate = DateTime.Now
                });

                int rowsAffected = _context.SaveChanges();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa xe thành công!");
                    LoadVehicles(); // Cập nhật danh sách xe
                    SelectedVehicle = null; // Xóa lựa chọn
                }
                else
                {
                    MessageBox.Show("Xóa xe thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa xe: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Back()
        {
            try
            {
                _mainViewModel.NavigateTo(new DashboardView { DataContext = new DashboardViewModel(_currentUserId, _mainViewModel) });
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
                _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    FilteredVehicles = new ObservableCollection<Vehicle>(Vehicles);
                else
                    FilteredVehicles = new ObservableCollection<Vehicle>(
                        Vehicles.Where(v => v.PlateNumber.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                          v.Brand.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                          v.Model.Contains(value, StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
                FilteredVehicles = new ObservableCollection<Vehicle>(Vehicles);
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