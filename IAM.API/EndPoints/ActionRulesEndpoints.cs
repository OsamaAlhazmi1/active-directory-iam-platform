using System;
using IAM_API.Auth;
using IAM_API.Data;
using IAM_API.DTOs;
using IAM_API.DTOs.RulesDTOs;
using IAM_API.Models;
using IAM_API.Services;
using Microsoft.EntityFrameworkCore;

namespace IAM_API.EndPoints;

public static class ActionRulesEndpoints
{
    const string GetActionRulesEndpointName = "GetLocalActionRule";


    public static void MapActionRulesEndpoints(this WebApplication app)
    {

        var actionRulesGroup = app.MapGroup("/actionRules");

        actionRulesGroup.MapPost("/", async (
        CreateActionRuleDTO dto,
        LocalContext dbContext,
        HttpContext http) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbContext);

            if (signedUser == null)
                return ApiResponse.Fail("No User is Signed", 409);
                
            if (signedUser.UserName != "IAM Admin")
                return ApiResponse.Fail("Only IAM Admin Can Make New Rule", 403);

            
            var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionName == dto.RequiredPermissionName);
            if (permission == null)
                return ApiResponse.Fail($"Required Permission  ({dto.RequiredPermissionName}) Not Found", 404);

            var alreadyExists = await dbContext.ActionRules.AnyAsync(r =>
            r.ActionType == dto.ActionType &&
            r.TargetType == dto.TargetType &&
            r.RequiredPermissionId == permission.Id);

            if (alreadyExists)
                return ApiResponse.Fail($"Rule Already exisit ", 401);

            ActionRule rule = new()
            {
                ActionType = dto.ActionType,
                TargetType = dto.TargetType,
                RequiredPermissionId = permission.Id,
                RequiredPermission = permission
            };

            await dbContext.ActionRules.AddAsync(rule);
            await dbContext.SaveChangesAsync();
            return ApiResponse.Success("Rule Successfuly Created", 201);


        }).RequireAuthorization();



        actionRulesGroup.MapGet("/", async (LocalContext dbcontext) =>
        {
            var allActionRules = await dbcontext.ActionRules
            .AsNoTracking()
            .Include(rule => rule.RequiredPermission)
            .Select(rule => new ActionRuleDetailsDTO(
                rule.Id,
                rule.ActionType.ToString(),
                rule.TargetType.ToString(),
                rule.RequiredPermission.PermissionName
            )).AsNoTracking().ToListAsync();

            return ApiResponse.Success("", allActionRules);

        });



    }






}
