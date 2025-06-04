using System.Net.Http.Json;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
using PumpStationManagemnet_BlazorApp.Request;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetUsersAsync(string? keyword = null)
        {
            var query = string.IsNullOrEmpty(keyword) ? "" : $"?keyword={keyword}";
            return await _httpClient.GetFromJsonAsync<List<User>>($"api/Users{query}") ?? new List<User>();
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/Users/{id}");
        }

        public async Task<HttpResponseMessage> CreateUserAsync(UserDTO userDto)
        {
            return await _httpClient.PostAsJsonAsync("api/Users", userDto);
        }

        public async Task<HttpResponseMessage> UpdateUserAsync(int id, UserDTO userDto)
        {
            return await _httpClient.PutAsJsonAsync($"api/Users/{id}", userDto);
        }

        public async Task<HttpResponseMessage> DeleteUserAsync(int id, int modifiedBy)
        {
            return await _httpClient.DeleteAsync($"api/Users/{id}?modifiedBy={modifiedBy}");
        }

        public async Task<HttpResponseMessage> RegisterAsync(RegisterRequest request)
        {
            return await _httpClient.PostAsJsonAsync("api/Users/register", request);
        }

        public async Task<HttpResponseMessage> LoginAsync(LoginRequest request)
        {
            return await _httpClient.PostAsJsonAsync("api/Users/login", request);
        }
    }
}
