using System.Text.Json;
using Shared.Common.DTOs.Identity;

namespace Shared.Services
{
    /// <summary>
    /// IdentityService ile HTTP üzerinden iletişim kurmak için kullanılan client
    /// </summary>
    public class IdentityHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public IdentityHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Tek kullanıcı bilgisini getirir
        /// </summary>
        public async Task<UserDto?> GetUserAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(content, _jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Birden fazla kullanıcı bilgisini toplu olarak getirir
        /// </summary>
        public async Task<List<UserDto>> GetBulkUsersAsync(List<Guid> userIds)
        {
            try
            {
                var json = JsonSerializer.Serialize(userIds, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/users/bulk", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserDto>>(responseContent, _jsonOptions) ?? new List<UserDto>();
                }
                return new List<UserDto>();
            }
            catch
            {
                return new List<UserDto>();
            }
        }

        /// <summary>
        /// Kullanıcının rolünü kontrol eder
        /// </summary>
        public async Task<string?> GetUserRoleAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/role");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kullanıcının var olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> UserExistsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/exists");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return bool.Parse(content);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Aktif kullanıcıları getirir
        /// </summary>
        public async Task<List<UserDto>> GetActiveUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/users/active");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserDto>>(content, _jsonOptions) ?? new List<UserDto>();
                }
                return new List<UserDto>();
            }
            catch
            {
                return new List<UserDto>();
            }
        }

        /// <summary>
        /// Belirli şehirdeki kullanıcıları getirir
        /// </summary>
        public async Task<List<UserDto>> GetUsersByCityAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/by-city/{Uri.EscapeDataString(city)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<UserDto>>(content, _jsonOptions) ?? new List<UserDto>();
                }
                return new List<UserDto>();
            }
            catch
            {
                return new List<UserDto>();
            }
        }

        /// <summary>
        /// Kullanıcının admin olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> IsUserAdminAsync(Guid userId)
        {
            var role = await GetUserRoleAsync(userId);
            return role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
