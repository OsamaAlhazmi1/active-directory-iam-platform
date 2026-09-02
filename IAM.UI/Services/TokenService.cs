using Microsoft.JSInterop;

public class TokenService
{
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> Get()
    {
        try
        {
            return await _js.InvokeAsync<string>("auth.getToken");
        }
        catch
        {
            // 🔥 THIS FIXES PRERENDER CRASH
            return null;
        }
    }

    public async Task Set(string token)
    {
        await _js.InvokeVoidAsync("auth.setToken", token);
    }

    public async Task Clear()
    {
        try
        {
            await _js.InvokeVoidAsync("auth.clearToken");
        }
        catch
        {
            // ignore JS issues during prerender or reload
        }
    }
}