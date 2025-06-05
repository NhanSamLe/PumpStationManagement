using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
using System.Net.Http.Json;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class AlertService
    {
        private readonly HttpClient _httpClient;

        public AlertService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Alert>> GetAlertsAsync(string? keyword = null, int? stationId = null)
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
            return await _httpClient.GetFromJsonAsync<List<Alert>>($"api/Alerts{query}") ?? new List<Alert>();
        }

        public async Task<Alert?> GetAlertAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Alert>($"api/Alerts/{id}");
        }

        public async Task<HttpResponseMessage> CreateAlertAsync(AlertDTO alertDto)
        {
            return await _httpClient.PostAsJsonAsync("api/Alerts", alertDto);
        }

        public async Task<HttpResponseMessage> UpdateAlertAsync(int id, AlertDTO alertDto)
        {
            return await _httpClient.PutAsJsonAsync($"api/Alerts/{id}", alertDto);
        }

        public async Task<HttpResponseMessage> DeleteAlertAsync(int id, int modifiedBy)
        {
            return await _httpClient.DeleteAsync($"api/Alerts/{id}?modifiedBy={modifiedBy}");
        }

        public async Task<HttpResponseMessage> ResolveAlertAsync(int id, int modifiedBy)
        {
            return await _httpClient.PatchAsync($"api/Alerts/{id}/resolve?modifiedBy={modifiedBy}", null);
        }

        public async Task<HttpResponseMessage> IgnoreAlertAsync(int id, int modifiedBy)
        {
            return await _httpClient.PatchAsync($"api/Alerts/{id}/ignore?modifiedBy={modifiedBy}", null);
        }
    }
}
