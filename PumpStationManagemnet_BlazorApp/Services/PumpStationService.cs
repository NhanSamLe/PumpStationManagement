using System.Net.Http.Json;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class PumpStationService
    {
        private readonly HttpClient _httpClient;

        public PumpStationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PumpStation>> GetPumpStationsAsync(string? keyword = null)
        {
            var query = string.IsNullOrEmpty(keyword) ? "" : $"?keyword={keyword}";
            return await _httpClient.GetFromJsonAsync<List<PumpStation>>($"api/PumpStations{query}") ?? new List<PumpStation>();
        }

        public async Task<PumpStation?> GetPumpStationAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PumpStation>($"api/PumpStations/{id}");
        }

        public async Task<HttpResponseMessage> CreatePumpStationAsync(PumpStationDTO stationDto)
        {
            return await _httpClient.PostAsJsonAsync("api/PumpStations", stationDto);
        }

        public async Task<HttpResponseMessage> UpdatePumpStationAsync(int id, PumpStationDTO stationDto)
        {
            return await _httpClient.PutAsJsonAsync($"api/PumpStations/{id}", stationDto);
        }

        public async Task<HttpResponseMessage> DeletePumpStationAsync(int id, int modifiedBy)
        {
            return await _httpClient.DeleteAsync($"api/PumpStations/{id}?modifiedBy={modifiedBy}");
        }
    }
}
