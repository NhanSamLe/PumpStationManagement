using System.Net.Http.Json;
using PumpStationManagemnet_BlazorApp.DTOs;
using PumpStationManagemnet_BlazorApp.Models;
namespace PumpStationManagemnet_BlazorApp.Services
{
    public class AuditLogService
    {
        private readonly HttpClient _http;

        public AuditLogService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(string? entityType = null, int? entityId = null, string? actionType = null)
        {
            var url = "api/auditlogs";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(entityType)) queryParams.Add($"entityType={entityType}");
            if (entityId.HasValue) queryParams.Add($"entityId={entityId}");
            if (!string.IsNullOrEmpty(actionType)) queryParams.Add($"actionType={actionType}");
            if (queryParams.Any()) url += "?" + string.Join("&", queryParams);
            return await _http.GetFromJsonAsync<List<AuditLog>>(url) ?? new List<AuditLog>();
        }
    }
}
