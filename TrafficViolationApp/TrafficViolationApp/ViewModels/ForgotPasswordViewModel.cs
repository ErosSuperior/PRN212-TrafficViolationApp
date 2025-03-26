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
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TrafficViolationApp.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string otp = string.Empty;
        private string _generatedOtp = string.Empty;
        private readonly TrafficViolationDbContext _context;
        private readonly MainViewModel _mainViewModel;
        private readonly IConfiguration _configuration;

        public ForgotPasswordViewModel(MainViewModel mainViewModel)
        {
            _context = new TrafficViolationDbContext();
            _mainViewModel = mainViewModel;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
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

                _generatedOtp = new Random().Next(100000, 999999).ToString();
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "localhost";
                
                // Verificar y proporcionar valor predeterminado para el puerto SMTP
                int smtpPort = 587; // Puerto predeterminado común para SMTP
                if (!string.IsNullOrEmpty(_configuration["EmailSettings:SmtpPort"]))
                {
                    if (int.TryParse(_configuration["EmailSettings:SmtpPort"], out int parsedPort))
                    {
                        smtpPort = parsedPort;
                    }
                }
                
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "no-reply@example.com";
                var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? string.Empty;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TrafficViolationApp", senderEmail));
                message.To.Add(new MailboxAddress(user.FullName, Email));
                message.Subject = "Mã OTP đặt lại mật khẩu";
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
        private void ResetPassword(PasswordBox newPasswordBox)
        {
            var newPassword = newPasswordBox?.Password;
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Otp) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                MessageBox.Show("Email không tồn tại!");
                return;
            }

            if (Otp != _generatedOtp)
            {
                MessageBox.Show("OTP không đúng!");
                return;
            }

            user.Password = newPassword; // Lưu ý: Nên mã hóa trong thực tế
            int rowsAffected = _context.SaveChanges();
            if (rowsAffected > 0)
            {
                MessageBox.Show("Đặt lại mật khẩu thành công! Vui lòng đăng nhập.");
                _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại!");
            }
        }

        [RelayCommand]
        private void BackToLogin()
        {
            _mainViewModel.NavigateTo(new LoginView { DataContext = new LoginViewModel(_mainViewModel) });
        }
    }
}