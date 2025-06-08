using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
using PumpStationManagemnet_BlazorApp.Request;
using Microsoft.JSInterop;
using PumpStationManagemnet_BlazorApp.Enums;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private User? _currentUser;
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }
        public User? CurrentUser => _currentUser;
        public int? CurrentUserId => _currentUser?.UserId;
        public bool IsAuthenticated => _currentUser != null;
        // Đăng nhập
        public async Task<(bool success, string message, User? user)> LoginAsync(LoginRequest loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/login", loginDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var user = JsonSerializer.Deserialize<User>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (user != null)
                    {
                        _currentUser = user;

                        // Lưu thông tin user vào localStorage
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", user.UserId.ToString());
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "username", user.Username);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userFullName", user.FullName ?? "");
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", user.Role.ToString());

                        return (true, "Đăng nhập thành công", user);
                    }
                }

                // Xử lý lỗi từ server
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                var message = errorResponse?.GetValueOrDefault("message")?.ToString() ?? "Đăng nhập thất bại";

                return (false, message, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối: {ex.Message}", null);
            }
        }
        public async Task<(bool success, string message, User? user)> RegisterAsync(RegisterRequest registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Users/register", registerDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var user = JsonSerializer.Deserialize<User>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return (true, "Đăng ký thành công", user);
                }

                // Xử lý lỗi từ server
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                var message = errorResponse?.GetValueOrDefault("message")?.ToString() ?? "Đăng ký thất bại";

                return (false, message, null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối: {ex.Message}", null);
            }
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;

            // Xóa thông tin từ localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "username");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userFullName");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
        }

        public async Task<bool> RestoreSessionAsync()
        {
            try
            {
                var userIdStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out var userId))
                {
                    var username = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "username");
                    var fullName = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userFullName");
                    var roleStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");

                    if (!string.IsNullOrEmpty(username) && int.TryParse(roleStr, out var role))
                    {
                        // Tạo user object từ thông tin đã lưu
                        _currentUser = new User
                        {
                            UserId = userId,
                            Username = username,
                            FullName = fullName,
                            Role = role
                        };

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool HasRole(UserRole requiredRole)
        {
            return _currentUser != null && _currentUser.Role >= (int)requiredRole;
        }

        // Lấy thông tin user hiện tại từ server (nếu cần)
        public async Task<User?> GetCurrentUserFromServerAsync()
        {
            if (_currentUser == null) return null;

            try
            {
                var response = await _httpClient.GetAsync($"api/Users/{_currentUser.UserId}");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<User>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (user != null)
                    {
                        _currentUser = user;
                    }

                    return user;
                }
            }
            catch
            {
                // Ignore errors
            }

            return _currentUser;
        }

    }
}
