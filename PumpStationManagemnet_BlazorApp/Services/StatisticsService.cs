using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class StatisticsService
    {
        private readonly HttpClient _httpClient;

        public StatisticsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StationStatusDto>> GetStationStatusAsync(int? stationId = null)
        {
            try
            {
                var query = stationId.HasValue && stationId > 0 ? $"?stationId={stationId}" : "";
                var response = await _httpClient.GetAsync($"api/Statistics/station-status{query}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<List<StationStatusDto>>();
                return data ?? new List<StationStatusDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu thống kê: {ex.Message}");
                throw;
            }
        }

        public async Task<List<PumpStation>> GetStationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/PumpStations");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<List<PumpStation>>();
                return data ?? new List<PumpStation>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách trạm bơm: {ex.Message}");
                throw;
            }
        }
    }
}
