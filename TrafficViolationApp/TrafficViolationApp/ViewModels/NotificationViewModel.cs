using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Notification> notifications = new();
        [ObservableProperty] private int unreadNotificationCount;

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;
        private readonly string _userRole;

        public NotificationViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            _userRole = _context.Users.FirstOrDefault(u => u.UserId == userId)?.Role ?? "Citizen";
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            Notifications = new ObservableCollection<Notification>(_context.Notifications.Where(n => n.UserId == _currentUserId));
            UpdateUnreadCount();
        }

        private void UpdateUnreadCount()
        {
            UnreadNotificationCount = Notifications.Count(n => n.IsRead != true);
        }

        [RelayCommand]
        private void MarkAsRead()
        {
            foreach (var notification in Notifications.Where(n => n.IsRead != true))
            {
                notification.IsRead = true;
            }
            
            _context.SaveChanges();
            UpdateUnreadCount();
        }

        [RelayCommand]
        private void Back()
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
    }
}