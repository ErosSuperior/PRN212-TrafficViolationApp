using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty] private string fullName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string phone = string.Empty;
        [ObservableProperty] private string address = string.Empty;
        // Rol fijo como Citizen
        private const string ROLE = "Citizen";

        private readonly TrafficViolationDbContext _context;
        private readonly MainViewModel _mainViewModel;

        public RegisterViewModel(MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void SubmitRegister(PasswordBox passwordBox)
        {
            try
            {
                // Obtenemos la contraseña directamente del parámetro
                var password = passwordBox?.Password;

                // Comprobamos datos de entrada
                if (string.IsNullOrEmpty(FullName))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!");
                    return;
                }
                
                if (string.IsNullOrEmpty(Email))
                {
                    MessageBox.Show("Vui lòng nhập email!");
                    return;
                }
                
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!");
                    return;
                }
                
                if (string.IsNullOrEmpty(Phone))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!");
                    return;
                }
                
                if (string.IsNullOrEmpty(Address))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ!");
                    return;
                }

                if (_context.Users.Any(u => u.Email == Email))
                {
                    MessageBox.Show("Email đã tồn tại!");
                    return;
                }

                var newUser = new User
                {
                    FullName = FullName,
                    Email = Email,
                    Password = password, // Lưu ý: Nên mã hóa mật khẩu trong thực tế
                    Role = ROLE, // Siempre usamos rol Citizen
                    Phone = Phone,
                    Address = Address
                };

                _context.Users.Add(newUser);
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.");
                    _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error registering user: {ex.Message}");
            }
        }

        [RelayCommand]
        private void BackToLogin()
        {
            _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
        }
    }
}