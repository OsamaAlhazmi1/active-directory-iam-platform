using System;
using AD_web_project.DTOs.AD_DTOs;
using AD_web_project.Models;
using AD_web_project.Services;

namespace AD_web_project.EndPoints;

public static class ADEndpoints
{
    public static void MapADEndpoint(this WebApplication app)

    {
        var ADgroup = app.MapGroup("/ad");


        // geting the usere from the AD 
        ADgroup.MapGet("/users", async (IActiveDirectoryService adService) =>
        {
            var users = await adService.GetUsersAsync();
            return ApiResponse.Success("", users);
        });

        ADgroup.MapGet("/users/{userName}/details", async (string userName, IActiveDirectoryService adService) =>
        {
            var user = await adService.GetUserDetailsAsync(userName);
            if (user is null)
                return ApiResponse.Fail($"user{userName} not found", 404);

            return ApiResponse.Success("", user);

        });
        ADgroup.MapPost("/users/{username}/enable", async (string username, IActiveDirectoryService adService) =>
        {
            var succes = await adService.EnableUserAsync(username);
            if (succes == false)
                ApiResponse.Fail($"user {username} not found", 404);

            else
                ApiResponse.Success($"user {username}  enabled successfully", 200);
        });
        ADgroup.MapPost("/users/{username}/disable", async (string username, IActiveDirectoryService adService) =>
    {
        var succes = await adService.DisableUserAsync(username);
        if (succes == false)
            ApiResponse.Fail($"user {username} not found", 404);

        else
            ApiResponse.Success($"user {username}  disable successfully", 200);


    });
        ADgroup.MapPost("/users/{username}/reset-password", async (string username, ADRestPasswordUserDTO dto, IActiveDirectoryService adService) =>
        {
            var succes = await adService.ResetPasswordAsync(username, dto.NewPassword);
            if (succes == false)
                ApiResponse.Fail($"user {username} not found", 404);

            else
                ApiResponse.Success($"user {username}  Password successfully Rest", 200);

        });

    }


}
