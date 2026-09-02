using System;
using AD_web_project.Auth;
using AD_web_project.Data;
using AD_web_project.DTOs;
using AD_web_project.DTOs.RulesDTOs;
using AD_web_project.Models;
using AD_web_project.Services;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.EndPoints;

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
