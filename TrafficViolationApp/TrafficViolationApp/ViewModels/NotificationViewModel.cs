using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class NotificationViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] private ObservableCollection<Notification> notifications = new();
        [ObservableProperty] private int unreadNotificationCount;

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;
        private readonly string _userRole;
        private bool _disposed = false;

        public NotificationViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            _userRole = user?.Role ?? "Citizen";
            
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                var userNotifications = _context.Notifications?.Where(n => n.UserId == _currentUserId).ToList();
                Notifications = new ObservableCollection<Notification>(userNotifications ?? new List<Notification>());
                UpdateUnreadCount();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Không thể tải thông báo: {ex.Message}");
                Notifications = new ObservableCollection<Notification>();
                UnreadNotificationCount = 0;
            }
        }

        private void UpdateUnreadCount()
        {
            UnreadNotificationCount = Notifications?.Count(n => n.IsRead != true) ?? 0;
        }

        [RelayCommand]
        private void MarkAsRead()
        {
            try
            {
                foreach (var notification in Notifications.Where(n => n.IsRead != true))
                {
                    notification.IsRead = true;
                }
                
                _context.SaveChanges();
                UpdateUnreadCount();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Không thể đánh dấu đã đọc: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Back()
        {
            try
            {
                if (_userRole == "TrafficPolice")
                {
                    _mainViewModel.NavigateTo(new DashboardView { DataContext = new DashboardViewModel(_currentUserId, _mainViewModel) });
                }
                else
                {
                    _mainViewModel.NavigateTo(new ReportView { DataContext = new ReportViewModel(_currentUserId, _mainViewModel) });
                }
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