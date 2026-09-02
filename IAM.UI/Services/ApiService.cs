using System.Net.Http.Headers;

namespace IAM.UI.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly TokenService _token;

    public ApiService(IHttpClientFactory factory, TokenService token)
    {
        _http = factory.CreateClient("API"); // 🔥 IMPORTANT FIX
        _token = token;
    }

    private async Task AttachToken()
    {
        var token = await _token.Get();

        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        await AttachToken();
        return await _http.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object data)
    {
        await AttachToken();
        return await _http.PostAsJsonAsync(url, data);
    }
}