using System;
using Microsoft.Extensions.Options;

namespace AD_web_project.Auth;

public static class ServiceExtensions

{
    public static void AddJwtServices(this WebApplicationBuilder builder)
    {
        // Bind JwtOptions from appsettings.json
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Jwt"));

        // Register TokenService
        builder.Services.AddScoped<TokenService>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
            return new TokenService(options);
        });
    }
}
