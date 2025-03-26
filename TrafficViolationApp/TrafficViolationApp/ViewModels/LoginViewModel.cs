using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private bool usePassword = true; // Mặc định dùng mật khẩu
        [ObservableProperty] private bool useOtp;
        [ObservableProperty] private string otp = string.Empty;
        private string _generatedOtp = string.Empty; // Lưu OTP đã gửi
        private readonly TrafficViolationDbContext _context;
        private readonly MainViewModel _mainViewModel;
        private readonly IConfiguration _configuration; // Thêm field để lưu cấu hình

        public LoginViewModel(MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _mainViewModel = mainViewModel;

            // Khởi tạo IConfiguration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [RelayCommand]
        private void SendOtp()
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == Email);
                if (user == null)
                {
                    MessageBox.Show("Email không tồn tại!");
                    return;
                }

                // Tạo OTP ngẫu nhiên
                _generatedOtp = new Random().Next(100000, 999999).ToString();

                // Đọc thông tin email từ appsettings.json
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured");
                var smtpPortStr = _configuration["EmailSettings:SmtpPort"];
                if (string.IsNullOrEmpty(smtpPortStr) || !int.TryParse(smtpPortStr, out var smtpPort))
                {
                    throw new InvalidOperationException("Invalid SMTP port configuration");
                }
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? throw new InvalidOperationException("Sender email not configured");
                var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? throw new InvalidOperationException("Sender password not configured");

                // Gửi email chứa OTP
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TrafficViolationApp", senderEmail));
                message.To.Add(new MailboxAddress(user.FullName, Email));
                message.Subject = "Mã OTP đăng nhập";
                message.Body = new TextPart("plain") { Text = $"Mã OTP của bạn là: {_generatedOtp}. Mã này có hiệu lực trong 5 phút." };

                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(senderEmail, senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                MessageBox.Show("OTP đã được gửi đến email của bạn!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi OTP: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Login(PasswordBox passwordBox)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                MessageBox.Show("Email không tồn tại!");
                return;
            }

            if (UsePassword)
            {
                var password = passwordBox?.Password;
                if (user.Password == password)
                {
                    NavigateToRole(user);
                }
                else
                {
                    MessageBox.Show("Mật khẩu không đúng!");
                }
            }
            else if (UseOtp)
            {
                if (string.IsNullOrEmpty(Otp))
                {
                    MessageBox.Show("Vui lòng nhập OTP!");
                }
                else if (Otp == _generatedOtp)
                {
                    NavigateToRole(user);
                }
                else
                {
                    MessageBox.Show("OTP không đúng!");
                }
            }
        }

        [RelayCommand]
        private void Register()
        {
            _mainViewModel.NavigateTo(new RegisterView { DataContext = new RegisterViewModel(_mainViewModel) });
        }
        [RelayCommand]
        private void ForgotPassword()
        {
            _mainViewModel.NavigateTo(new ForgotPasswordView { DataContext = new ForgotPasswordViewModel(_mainViewModel) });
        }
        private void NavigateToRole(User user)
        {
            if (user.Role == "Citizen")
                _mainViewModel.NavigateTo(new ReportView { DataContext = new ReportViewModel(user.UserId, _mainViewModel) });
            else if (user.Role == "TrafficPolice")
                _mainViewModel.NavigateTo(new DashboardView { DataContext = new DashboardViewModel(user.UserId, _mainViewModel) });
        }
    }
}