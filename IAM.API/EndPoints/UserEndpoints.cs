using System;
using AD_web_project.Auth;
using AD_web_project.Data;
using AD_web_project.DTOs;
using AD_web_project.DTOs.AuthDTOs;
using AD_web_project.Models;
using AD_web_project.Services;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.EndPoints;

public static class UserEndpoints
{
    const string GetAllLocalUsersEndpointName = "GetLocalUser";

    public static void MapUserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/users");


        ///////////////Registering new Local User EndPoints
        app.MapPost("/register", async (CreateUserDTO dto, LocalContext dbcontext) =>
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ApiResponse.Fail("User Name can't be null or empty", 409);

            var normalized = dto.UserName.Trim().ToLower();
            bool exists = await dbcontext.Users.AnyAsync(u => u.UserName.Trim().ToLower() == normalized);
            if (exists)
                return ApiResponse.Fail($"User {dto.UserName} Already Exsist", 409);

            User newUser = new()
            {
                UserName = dto.UserName,
                /// very importent to change later , as if plain text in the lacal database 
                Password = dto.Password

            };

            await dbcontext.Users.AddAsync(newUser);
            await dbcontext.SaveChangesAsync();

            return ApiResponse.Success($"User {dto.UserName} Successfully Created", 201);

        });


        /////////// Login the user 

        app.MapPost("/login", async (
        LoginDTO dto,
        LocalContext dbcontext, 
        TokenService tokenService) =>
        {

            var user = await dbcontext.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user == null || user.Password != dto.Password)
                return ApiResponse.Fail("UserName or Password Wrong", 400);

            else
            {
                var tokken = tokenService.CreateToken(user.Id, user.UserName);
                return ApiResponse.Success("Login Success", tokken);
            }

        });

        //get all users(just cards)
        userGroup.MapGet("/", async (LocalContext dbcontext) =>
        {
            var allLocalUsers = await dbcontext.Users.Select(user =>
            new UserSummaryDTO(
               user.Id, user.UserName
            )).AsNoTracking().ToListAsync();

            return ApiResponse.Success("", allLocalUsers);

        }).WithName(GetAllLocalUsersEndpointName);



        //get user by id (just card )
        userGroup.MapGet("/{id:int}", async (int id, LocalContext dbcontext) =>
        {
            var user = await dbcontext.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return ApiResponse.Fail($"User With {id} Not Found", 404);

            var dto = new UserSummaryDTO(
                user.Id,
                user.UserName
            );

            return ApiResponse.Success("", dto);
        });



        //get user by Name(just card)
        userGroup.MapGet("/by-name/{userName}", async (string userName, LocalContext dbcontext) =>
        {
            var user = await dbcontext.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return ApiResponse.Fail($"User With {userName} Not Found", 404);

            var dto = new UserSummaryDTO(
                user.Id,
                user.UserName
            );

            return ApiResponse.Success("", dto);
        });



        // get details of User (user name + user id + all of permesions)
        userGroup.MapGet("/details-user/{id}", async (int id, LocalContext dbcontext) =>
        {
            var user = await dbcontext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDetailsDTO(
                    u.Id,
                    u.UserName,

                    // PERMISSIONS
                    u.UserPermissions
                        .Select(up => up.Permission.PermissionName)
                        .ToArray(),

                    // GROUPS 
                    u.GroupUsers
                        .Select(g => g.Group.GroupName)
                        .ToArray()
                ))
                .FirstOrDefaultAsync();

            if (user == null)
                return ApiResponse.Fail($"User With id {id} Not Found", 404);

            return ApiResponse.Success("", user);
        });



        /// me 
        app.MapGet("/me", async (LocalContext dbcontext, HttpContext http) =>
        {
            var userIdClaim = http.User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return ApiResponse.Fail("User Unauthorized ", 401);

            var userId = int.Parse(userIdClaim);

            var user = await dbcontext.Users
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
                .Include(u => u.GroupUsers)
                .ThenInclude(u => u.Group)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            var groups = await dbcontext.GroupUsers
            .Where(gu => gu.UserId == userId)
            .Include(gu => gu.Group)
            .Select(gu => gu.Group.GroupName)
            .ToListAsync();


            if (user is null)
                return ApiResponse.Fail("User Not Found ", 404);

            var result = new MeDto(
                user.Id,
                user.UserName,
                user.UserPermissions
                    .Select(up => up.Permission.PermissionName)
                    .ToList(),
                groups
                

            );

            return ApiResponse.Success("", result);
        }).RequireAuthorization();
    }






}
