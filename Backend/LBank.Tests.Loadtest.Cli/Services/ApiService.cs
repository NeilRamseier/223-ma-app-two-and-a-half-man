using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bank.Core.Models;

namespace LBank.Tests.Loadtest.Cli.Services;

public class ApiService
{
    private string _apiUrl = "http://localhost:5000/api/v1";
    private HttpClient _client = new HttpClient();

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public async Task<List<Ledger>> GetLedgers(string jwt)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var response = await _client.GetAsync(_apiUrl + "/ledgers");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Unable to get ledgers");
        }

        var ledgers = await response.Content.ReadFromJsonAsync<List<Ledger>>();

        return ledgers ?? new List<Ledger>();
    }

    private class User
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public async Task<String> Login(string username, string password)
    {
        User user = new User { Username = username, Password = password };


        HttpResponseMessage response = await _client.PostAsJsonAsync(_apiUrl + "/login", user);

        var jwtToken = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return jwtToken.Token;
    }
}