﻿
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
using System.Net.Http.Json;

namespace PumpStationManagemnet_BlazorApp.Services
{
    public class OperatingService
    {
        private readonly HttpClient _httpClient;

        public OperatingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OperatingData>> GetOperatingDataAsync(string? keyword = null, int? stationId = null)
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
            return await _httpClient.GetFromJsonAsync<List<OperatingData>>($"api/Operatings{query}") ?? new List<OperatingData>();
        }

        public async Task<OperatingData?> GetOperatingDatumAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<OperatingData>($"api/Operatings/{id}");
        }

        public async Task<HttpResponseMessage> CreateOperatingDatumAsync(OperatingDataDTO operatingDataDto)
        {
            return await _httpClient.PostAsJsonAsync("api/Operatings", operatingDataDto);
        }

        public async Task<HttpResponseMessage> UpdateOperatingDatumAsync(int id, OperatingDataDTO operatingDataDto)
        {
            return await _httpClient.PutAsJsonAsync($"api/Operatings/{id}", operatingDataDto);
        }

        public async Task<HttpResponseMessage> DeleteOperatingDatumAsync(int id, int modifiedBy)
        {
            return await _httpClient.DeleteAsync($"api/Operatings/{id}?modifiedBy={modifiedBy}");
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
            return $"api/Operatings/export-excel{query}";
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
               
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi API xuất Excel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
