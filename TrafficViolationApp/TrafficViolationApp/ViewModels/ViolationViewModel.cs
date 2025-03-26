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
    public partial class ViolationViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Violation> violations = new();
        [ObservableProperty] private Violation? selectedViolation;

        private readonly TrafficViolationDbContext _context;
        private readonly int _currentUserId;
        private readonly MainViewModel _mainViewModel;

        public ViolationViewModel(int userId, MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _currentUserId = userId;
            _mainViewModel = mainViewModel;
            LoadViolations();
        }

        private void LoadViolations()
        {
            try
            {
                Violations = new ObservableCollection<Violation>(
                    _context.Violations
                        .Where(v => v.ViolatorId == _currentUserId)
                        .ToList()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vi phạm: {ex.Message}");
            }
        }

        [RelayCommand]
        private void PayFine()
        {
            try
            {
                if (SelectedViolation == null)
                {
                    MessageBox.Show("Vui lòng chọn một vi phạm để nộp phạt!");
                    return;
                }

                // Kiểm tra PaidStatus (bool?) - sử dụng .HasValue và .Value để xử lý nullable
                if (SelectedViolation.PaidStatus.HasValue && SelectedViolation.PaidStatus.Value)
                {
                    MessageBox.Show("Vi phạm này đã được nộp phạt trước đó!");
                    return;
                }

                // Gán giá trị true cho PaidStatus
                SelectedViolation.PaidStatus = true;

                // Lưu thay đổi vào database
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Đã nộp phạt thành công!");
                    LoadViolations(); // Tải lại danh sách để cập nhật giao diện
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào được lưu!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nộp phạt: {ex.Message}");
                LoadViolations(); // Rollback giao diện nếu có lỗi
            }
        }

        [RelayCommand]
        private void Back()
        {
            try
            {
                _mainViewModel.NavigateTo(new ReportView { DataContext = new ReportViewModel(_currentUserId, _mainViewModel) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}");
            }
        }
    }
}