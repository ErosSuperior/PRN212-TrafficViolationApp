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
    public partial class UserListViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] private ObservableCollection<User> users = new();
        [ObservableProperty] private ObservableCollection<User> filteredUsers = new();
        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private int maxUsersToDisplay = 100; // Giới hạn số lượng người dùng hiển thị

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;
        private bool _disposed = false;

        public UserListViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            LoadUsers();
        }

        partial void OnSearchTextChanged(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    FilteredUsers = new ObservableCollection<User>(Users);
                else
                    FilteredUsers = new ObservableCollection<User>(
                        Users.Where(u => u.FullName.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                        u.Email.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                        u.Phone.Contains(value, StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
                FilteredUsers = new ObservableCollection<User>(Users);
            }
        }

        private void LoadUsers()
        {
            try
            {
                // Giới hạn số lượng người dùng được tải để tránh vấn đề hiệu suất
                var userList = _context.Users?.Take(MaxUsersToDisplay).ToList();
                Users = new ObservableCollection<User>(userList ?? new List<User>());
                FilteredUsers = new ObservableCollection<User>(Users); // Ban đầu hiển thị tất cả
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách người dùng: {ex.Message}");
                Users = new ObservableCollection<User>();
                FilteredUsers = new ObservableCollection<User>();
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