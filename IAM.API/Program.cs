using IAM_API.Auth;
using IAM_API.Data;
using IAM_API.EndPoints;
using IAM_API.Services;
using IAM_API.Services.AccessControl;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddLocalDb();
builder.AddJwtAuthentication();
builder.AddJwtServices();
builder.Services.AddScoped<IAccessControlService, AccessControlService>();
builder.Services.AddScoped<IActiveDirectoryService,ActiveDirectoryService>();
builder.Services.AddValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("blazor", policy =>
    {
        policy
            .WithOrigins("https://localhost:5298", "http://localhost:5280")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("blazor");
app.MigrateDb(); 
app.MapPermissionEndpoints(); 
app.MapUserEndpoints();
app.MapComputerEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapActionLogsEndpoints();
app.MapActionRulesEndpoints();
app.MapGroupEndpoints();
app.MapADEndpoint();

app.Run();
