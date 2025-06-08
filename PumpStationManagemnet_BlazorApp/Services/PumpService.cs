using System.Net.Http.Json;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class PumpService
    {
     
            private readonly HttpClient _httpClient;

            public PumpService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<List<Pump>> GetPumpsAsync(string? keyword = null, int? stationId = null)
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
                return await _httpClient.GetFromJsonAsync<List<Pump>>($"api/Pumps{query}") ?? new List<Pump>();
            }

            public async Task<Pump?> GetPumpAsync(int id)
            {
                return await _httpClient.GetFromJsonAsync<Pump>($"api/Pumps/{id}");
            }

            public async Task<HttpResponseMessage> CreatePumpAsync(PumpDTO pumpDto)
            {
                return await _httpClient.PostAsJsonAsync("api/Pumps", pumpDto);
            }

            public async Task<HttpResponseMessage> UpdatePumpAsync(int id, PumpDTO pumpDto)
            {
                return await _httpClient.PutAsJsonAsync($"api/Pumps/{id}", pumpDto);
            }

            public async Task<HttpResponseMessage> DeletePumpAsync(int id, int modifiedBy)
            {
                return await _httpClient.DeleteAsync($"api/Pumps/{id}?modifiedBy={modifiedBy}");
            }
        public async Task<HttpResponseMessage> SetAllPumpsActiveAsync(int modifiedBy)
        {
            return await _httpClient.PutAsync($"api/Pumps/set-active-all?modifiedBy={modifiedBy}", null);
        }

        public async Task<HttpResponseMessage> SetAllPumpsInactiveAsync(int modifiedBy)
        {
            return await _httpClient.PutAsync($"api/Pumps/set-inactive-all?modifiedBy={modifiedBy}", null);
        }
        public string GetExportExcelUrl(string? keyword = null, int? stationId = null)
        {
            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryParts.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
            }

            if (stationId.HasValue && stationId > 0)
            {
                queryParts.Add($"stationId={stationId.Value}");
            }

            var query = queryParts.Any() ? $"?{string.Join("&", queryParts)}" : "";
            return $"api/Pumps/export-excel{query}";
        }

        public async Task<byte[]> ExportToExcel(string? keyword = null, int? stationId = null)
        {
            try
            {
                var relativeUrl = GetExportExcelUrl(keyword, stationId);
            
                var response = await _httpClient.GetAsync(relativeUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                 
                    throw new HttpRequestException($"Lỗi {response.StatusCode}: {errorContent}");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                Console.WriteLine($"Tải được {memoryStream.Length} bytes");
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API xuất Excel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw; // Ném lại ngoại lệ để xử lý ở phía gọi
            }
        }

    }
    }
