using System;
using AD_web_project.Auth;
using AD_web_project.Data;
using AD_web_project.DTOs;
using AD_web_project.DTOs.PermissionDTOs;
using AD_web_project.Models;
using AD_web_project.Services;
using AD_web_project.Services.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.EndPoints;

public static class PermissionEndpoints
{
    const string GetAllLocalPermissionEndpointName = "GetPermissions";

    public static void MapPermissionEndpoints(this WebApplication app)
    {
        var permissionGroup = app.MapGroup("/permissions");


        ///adding new premssion
        permissionGroup.MapPost("/", async (
        CreatePremissionDTO dto,
        LocalContext dbcontext,
        HttpContext http,
        IAccessControlService controlService) =>
        {

            var signedUser = await http.GetCurrentUserAsync(dbcontext);

            if (signedUser == null)
                  return ApiResponse.Fail("No User is Signed", 409);


            if (string.IsNullOrWhiteSpace(dto.PermissionName))
                return ApiResponse.Fail("Permission Name can't be null or empty", 409);

            var normalized = dto.PermissionName.Trim().ToLower();
            bool exists = await dbcontext.Permissions
                .AnyAsync(p => p.PermissionName.Trim().ToLower() == normalized);

            if (exists)
                return ApiResponse.Fail($"Permission ({dto.PermissionName}) Already Exisit", 409);



            var ruleActionType = ActionRule.RuleActionType.AssignPermission;
            var ruleTargetType = ActionRule.RuleTargetType.User;


            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);

            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);



            Permission newPermission = new()
            {
                PermissionName = dto.PermissionName,
                Permission_Type = dto.Permission_Type
            };

            await dbcontext.Permissions.AddAsync(newPermission);
            await dbcontext.SaveChangesAsync();


            //log the action 

            ActionLog log = new ActionLog()
            {
                TargetPermissionId = newPermission.Id,
                TargetPermission = newPermission,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.CreatePermission.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync();

            return ApiResponse.Success($"Permssion ({dto.PermissionName}) is Created Successfuly", 201);

        }).RequireAuthorization();



        //show permisson cards 
        permissionGroup.MapGet("/", async (LocalContext dbcontext) =>
        {
            var allLocalPermissions = await dbcontext.Permissions.Select(permission => new PermissionSummaryDTO(
               permission.Id, permission.PermissionName, permission.Permission_Type.ToString()
            )).AsNoTracking().ToListAsync();

            return ApiResponse.Success("", allLocalPermissions);

        }).WithName(GetAllLocalPermissionEndpointName);

        //assing permssion to user
        permissionGroup.MapPost("/{userId}/add-permission-user", async (
        int userId,
        AddToPremissionDTO dto,
        LocalContext dbcontext,
        HttpContext http,
        IAccessControlService controlService) =>
        {

            var signedUser = await http.GetCurrentUserAsync(dbcontext);

            if (signedUser == null)
                  return ApiResponse.Fail("No User is Signed", 409);


            var ruleActionType = ActionRule.RuleActionType.AssignPermission;
            var ruleTargetType = ActionRule.RuleTargetType.User;


            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);

            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);


            var user = await dbcontext.Users.FindAsync(userId);
            if (user == null)
                return ApiResponse.Fail($"Failed To Find User With Id ({userId}) ,(Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);

            var permission = await dbcontext.Permissions.FirstOrDefaultAsync(p => p.PermissionName == dto.PermissionName);
            if (permission is null)
                return ApiResponse.Fail($"Failed To Find Permssion {dto.PermissionName}", 404);

            if (permission.Permission_Type != Permission.PermissionType.User)
                return ApiResponse.Fail($"The Permisson ({permission.PermissionName}) Cant Be Assingend To a ({user.UserName})", 400);

            bool alreadyAssigned = await dbcontext.UserPermissions
            .AnyAsync(up => up.UserId == user.Id && up.PermissionId == permission.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"Permisson ({permission.PermissionName} )Alreday Assigned To ({user.UserName}) ", 400);

            UserPermission userPermission = new()
            {
                UserId = user.Id,
                PermissionId = permission.Id
            };
            await dbcontext.UserPermissions.AddAsync(userPermission);
            await dbcontext.SaveChangesAsync();


            //log the action 

            ActionLog log = new()
            {
                TargetUserId = user.Id,
                TargetUser = user,
                TargetPermissionId = permission.Id,
                TargetPermission = permission,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.AssignPermission.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync();
            return ApiResponse.Success($"Permisson ({permission.PermissionName}) Assigned To ({user.UserName}) Successfully", 201);

        }).RequireAuthorization();




        //get permssion by ID
        //get permssion by name


        //asigning premssion to a computer 
        permissionGroup.MapPost("/{computerId}/add-permission-computer", async (
        int computerId,
        AddToPremissionDTO dto,
        LocalContext dbcontext,
        HttpContext http,
        IAccessControlService controlService) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbcontext);

            if (signedUser == null)
                  return ApiResponse.Fail("No User is Signed", 409);


            var ruleActionType = ActionRule.RuleActionType.AssignPermission;
            var ruleTargetType = ActionRule.RuleTargetType.Computer;

            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);

            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);


            var computer = await dbcontext.Computers.FindAsync(computerId);
            if (computer == null)
                return ApiResponse.Fail($"Failed To Find Computer With ID ({computerId}) (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);


            var permission = await dbcontext.Permissions.FirstOrDefaultAsync(p => p.PermissionName == dto.PermissionName);

            if (permission == null)
                return ApiResponse.Fail($"Failed To Find Permssion {dto.PermissionName}", 404);

            if (permission.Permission_Type != Permission.PermissionType.Computer)
                return ApiResponse.Fail($"The Permisson ({permission.PermissionName}) Cant Be Assingend To a ({computer.ComputerName})", 400);

            bool alreadyAssigned = await dbcontext.ComputerPermissions
            .AnyAsync(up => up.ComputerId == computer.Id && up.PermissionId == permission.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"Permisson ({permission.PermissionName}) Alreday Assigned To ({computer.ComputerName}) ", 400);

            ComputerPermission computerPermission = new()
            {
                ComputerId = computer.Id,
                PermissionId = permission.Id
            };
            await dbcontext.ComputerPermissions.AddAsync(computerPermission);
            await dbcontext.SaveChangesAsync();

            //log the action 

            ActionLog log = new()
            {
                TargetComputerId = computer.Id,
                TargetComputer = computer,
                TargetPermissionId = permission.Id,
                TargetPermission = permission,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.AssignPermission.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync();
            return ApiResponse.Success($"Permisson ({permission.PermissionName}) Assigned To ({computer.ComputerName}) Successfully", 201);

        }).RequireAuthorization();

        // asigning permssion to group 

        permissionGroup.MapPost("/{groupId}/add-permission-group", async (
        int groupId,
        AddToPremissionDTO dto,
        LocalContext dbcontext,
        IAccessControlService controlService
        , HttpContext http) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbcontext);

            if (signedUser == null)
                  return ApiResponse.Fail("No User is Signed", 409);


            var ruleActionType = ActionRule.RuleActionType.AssignPermission;
            var ruleTargetType = ActionRule.RuleTargetType.Group;

            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);
            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);


            var group = await dbcontext.Groups.FindAsync(groupId);
            if (group == null)
                return ApiResponse.Fail($"Failed To Find Computer With ID ({groupId}) (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);


            var permission = await dbcontext.Permissions.FirstOrDefaultAsync(p => p.PermissionName == dto.PermissionName);

            if (permission == null)
                return ApiResponse.Fail($"Failed To Find Permssion {dto.PermissionName}", 404);

            if (permission.Permission_Type != Permission.PermissionType.Group)
                return ApiResponse.Fail($"The Permisson ({permission.PermissionName}) Cant Be Assingend To a ({group.GroupName})", 400);

            bool alreadyAssigned = await dbcontext.GroupPermissions
            .AnyAsync(up => up.GroupId == group.Id && up.PermissionId == permission.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"Permisson ({permission.PermissionName}) Alreday Assigned To ({group.GroupName}) ", 400);

            GroupPermission newGroupPermission = new()
            {
                GroupId = group.Id,
                PermissionId = permission.Id

            };
            await dbcontext.GroupPermissions.AddAsync(newGroupPermission);
            await dbcontext.SaveChangesAsync();

            //log the action 

            ActionLog log = new()
            {
                TargetGroupId = group.Id,
                TargetGroup = group,
                TargetPermissionId = permission.Id,
                TargetPermission = permission,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.AssignPermission.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync();

            return ApiResponse.Success($"Permisson ({permission.PermissionName}) Assigned To ({group.GroupName}) Successfully", 201);


        });









    }






}
