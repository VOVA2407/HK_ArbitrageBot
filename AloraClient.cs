using HK_ArbitrageBot.Models;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HK_ArbitrageBot;

internal class AloraClient : IDisposable
{
    private HttpClient _client;
    private string _accessToken;
    public AloraClient() {

        _client = new HttpClient();

        _client.BaseAddress = new Uri("https://apidev.alor.ru/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _accessToken = ConfigurationManager.AppSettings.Get("AlorToken") ??
            throw new Exception ("Не хватает токена для доступа к api алок. См. файл app.config");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {_accessToken}");

    }

    public void Authorize()
    {
        
    }

    private async Task<string> GetRefreshToken()   
    {
        string refreshToken = ConfigurationManager.AppSettings.Get("RefreshToken") ??
            throw new Exception("Не хватает токена для доступа к api алок. Поле RefreshToken. См. файл app.config");
        return await _client.GetStringAsync($"/refresh?token={refreshToken}");
    }

    public async Task<List<Security>> GetSecurities()
    {
        string request = "/md/v2/Securities/MOEX";
        return JsonSerializer.Deserialize <List<Security>>(await SendRequest(request));
    }

    public async Task<Security> GetQuotes(Security security)
    {
        string request = $"/md/v2/Securities/{security.Exchange}:{security.Symbol}/quotes";
        return JsonSerializer.Deserialize<Security>(await SendRequest(request));
    }

    public async Task<string> SendRequest(string request)
    {
        var response = await _client.GetStringAsync($"request");
        HttpResponseMessage message = JsonSerializer.Deserialize<HttpResponseMessage>(response);
        
        if(message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _accessToken = await GetRefreshToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {_accessToken}");
        }

        return await _client.GetStringAsync($"request");
    }

    

    public void Dispose()
    {
        _client.Dispose();
    }
}
