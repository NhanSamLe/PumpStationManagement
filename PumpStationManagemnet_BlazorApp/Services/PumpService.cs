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

        }
    }
