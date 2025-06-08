using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
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
        public string GetExportExcelUrl(string? keyword = null)
        {
            var query = string.IsNullOrWhiteSpace(keyword) ? "" : $"?keyword={Uri.EscapeDataString(keyword)}";
            return $"api/PumpStations/export-excel{query}";
        }

        public async Task<byte[]> ExportToExcel(string? keyword = null)
        {
            try
            {
                var relativeUrl = GetExportExcelUrl(keyword);
                var response = await _httpClient.GetAsync(relativeUrl);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API xuất Excel: {ex.Message}");
                throw; // Ném lại ngoại lệ để xử lý ở phía gọi
            }
        }
    }
}
