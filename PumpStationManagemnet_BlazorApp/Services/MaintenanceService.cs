using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
using System.Net.Http.Json;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class MaintenanceService
    {
        private readonly HttpClient _httpClient;

        public MaintenanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MaintenanceHistory>> GetMaintenanceHistoriesAsync(string? keyword = null, int? stationId = null)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(keyword) || stationId.HasValue)
            {
                var parameters = new List<string>();
                if (!string.IsNullOrEmpty(keyword))
                {
                    parameters.Add($"keyword={Uri.EscapeDataString(keyword)}");
                }
                if (stationId.HasValue)
                {
                    parameters.Add($"stationId={stationId.Value}");
                }
                query = $"?{string.Join("&", parameters)}";
            }
            return await _httpClient.GetFromJsonAsync<List<MaintenanceHistory>>($"api/MaintenanceHistory{query}") ?? new List<MaintenanceHistory>();
        }

        public async Task<MaintenanceHistory?> GetMaintenanceHistoryAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<MaintenanceHistory>($"api/MaintenanceHistory/{id}");
        }

        public async Task<HttpResponseMessage> CreateMaintenanceHistoryAsync(MaintenanceHistoryDTO maintenanceHistoryDto)
        {
            return await _httpClient.PostAsJsonAsync("api/MaintenanceHistory", maintenanceHistoryDto);
        }

        public async Task<HttpResponseMessage> UpdateMaintenanceHistoryAsync(int id, MaintenanceHistoryDTO maintenanceHistoryDto)
        {
            return await _httpClient.PutAsJsonAsync($"api/MaintenanceHistory/{id}", maintenanceHistoryDto);
        }

        public async Task<HttpResponseMessage> DeleteMaintenanceHistoryAsync(int id, int modifiedBy)
        {
            return await _httpClient.DeleteAsync($"api/MaintenanceHistory/{id}?modifiedBy={modifiedBy}");
        }

        public async Task<HttpResponseMessage> CompleteMaintenanceHistoryAsync(int id, int modifiedBy)
        {
            return await _httpClient.PatchAsync($"api/MaintenanceHistory/{id}/complete?modifiedBy={modifiedBy}", null);
        }
        public async Task<HttpResponseMessage> ActiveMaintenanceHistoryAsync(int id, int modifiedBy)
        {
            return await _httpClient.PatchAsync($"api/MaintenanceHistory/{id}/active?modifiedBy={modifiedBy}", null);
        }
    }
}
