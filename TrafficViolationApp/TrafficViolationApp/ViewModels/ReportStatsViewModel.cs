using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViolationApp.Models;
using TrafficViolationApp.Views;

namespace TrafficViolationApp.ViewModels
{
    public partial class ReportStatsViewModel : ObservableObject
    {
        [ObservableProperty] private string violationCountByArea = string.Empty;
        [ObservableProperty] private string violationCountByType = string.Empty;
        [ObservableProperty] private string reportStatusCount = string.Empty;

        private readonly TrafficViolationDbContext _context;
        private readonly MainViewModel _mainViewModel;
        private readonly int _currentUserId; 

        public ReportStatsViewModel(MainViewModel mainViewModel, int userId)
        {
            _context = new TrafficViolationDbContext();
            _mainViewModel = mainViewModel;
            _currentUserId = userId; 
            LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                // Cargar todos los reportes primero para hacer el procesamiento del lado del cliente
                var allReports = _context.Reports.ToList();
                
                // Estadísticas por área (khu vực)
                StringBuilder areaStatsBuilder = new StringBuilder();
                if (allReports.Any())
                {
                    var areaGroups = allReports.GroupBy(r => r.Location).ToList();
                    bool isFirst = true;
                    
                    foreach (var group in areaGroups)
                    {
                        if (!isFirst)
                            areaStatsBuilder.Append("\n");
                            
                        areaStatsBuilder.Append($"• {group.Key}: {group.Count()}");
                        isFirst = false;
                    }
                }
                else
                {
                    areaStatsBuilder.Append("Chưa có dữ liệu");
                }
                ViolationCountByArea = $"Vi phạm theo khu vực:\n{areaStatsBuilder}";
                
                // Estadísticas por tipo - Usando un enfoque simplificado para evitar problemas de codificación
                if (allReports.Any())
                {
                    var typeGroups = allReports.GroupBy(r => r.ViolationType).ToList();
                    
                    // Construir el texto manualmente, evitando el StringBuilder para esta sección
                    string typeText = "Vi phạm theo loại:";
                    
                    foreach (var group in typeGroups)
                    {
                        typeText += $"\n• {group.Key}: {group.Count()}";
                    }
                    
                    ViolationCountByType = typeText;
                }
                else
                {
                    ViolationCountByType = "Vi phạm theo loại:\nChưa có dữ liệu";
                }
                
                // Conteo de estados de reportes
                int pendingCount = allReports.Count(r => r.Status == "Pending");
                int approvedCount = allReports.Count(r => r.Status == "Approved");
                int rejectedCount = allReports.Count(r => r.Status == "Rejected");
                
                ReportStatusCount = $"Trạng thái phản ánh:\n• Chờ duyệt: {pendingCount}\n• Đã duyệt: {approvedCount}\n• Đã từ chối: {rejectedCount}";
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}");
                // Establecer mensajes predeterminados en caso de error
                ViolationCountByArea = "Vi phạm theo khu vực: Không thể tải dữ liệu";
                ViolationCountByType = "Vi phạm theo loại: Không thể tải dữ liệu";
                ReportStatusCount = "Trạng thái phản ánh: Không thể tải dữ liệu";
            }
        }

        [RelayCommand]
        private void Back()
        {
            _mainViewModel.NavigateTo(new DashboardView { DataContext = new DashboardViewModel(_currentUserId, _mainViewModel) });
        }
    }
}