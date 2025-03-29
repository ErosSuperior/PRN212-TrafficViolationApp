using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class MyVehiclesViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] private ObservableCollection<Vehicle> vehicles = new();

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;
        private bool _disposed = false;

        public MyVehiclesViewModel(int userId, MainViewModel mainViewModel)
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
                var userVehicles = _context.Vehicles.Where(v => v.OwnerId == _currentUserId).ToList();
                Vehicles = new ObservableCollection<Vehicle>(userVehicles ?? new List<Vehicle>());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Không thể tải danh sách xe: {ex.Message}");
                Vehicles = new ObservableCollection<Vehicle>();
            }
        }

        [RelayCommand]
        private void Back()
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == _currentUserId);
                if (user == null)
                {
                    System.Windows.MessageBox.Show("Không tìm thấy thông tin người dùng!");
                    return;
                }

                if (user.Role == "Citizen")
                    _mainViewModel.NavigateTo(new ReportView { DataContext = new ReportViewModel(_currentUserId, _mainViewModel) });
                else if (user.Role == "TrafficPolice")
                    _mainViewModel.NavigateTo(new DashboardView { DataContext = new DashboardViewModel(_currentUserId, _mainViewModel) });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
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