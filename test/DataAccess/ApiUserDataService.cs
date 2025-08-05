using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using test.Models;
using test.Services;

namespace test.DataAccess
{
    public class ApiUserDataService : IUserDataService
    {
        private readonly HttpClient _client;

        public ApiUserDataService(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL required", nameof(baseUrl));

            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<bool> AddUserAsync(UserModel user)
        {
            var response = await _client.PostAsJsonAsync("/users", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var response = await _client.DeleteAsync($"/users/{Uri.EscapeDataString(username)}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var response = await _client.PutAsJsonAsync($"/users/{user.Id}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = await _client.GetFromJsonAsync<List<UserModel>>("/users");
            return users ?? new List<UserModel>();
        }

        public async Task<UserModel?> GetUserAsync(string username)
        {
            return await _client.GetFromJsonAsync<UserModel>($"/users/{Uri.EscapeDataString(username)}");
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await GetUserAsync(username) != null;
        }

        public async Task<bool> VerifyUserPasswordAsync(string username, string password)
        {
            var response = await _client.PostAsJsonAsync("/login", new UserModel { Username = username, Password = password });
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            return result?.success ?? false;
        }

        public async Task<bool> UpdateUserDescriptionAsync(int id, string description)
        {
            var response = await _client.PutAsync($"/users/{id}/description?description={Uri.EscapeDataString(description)}", null);
            return response.IsSuccessStatusCode;
        }

        private record LoginResult(bool success);
    }
}
