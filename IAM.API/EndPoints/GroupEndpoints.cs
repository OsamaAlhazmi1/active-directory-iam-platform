using System;
using AD_web_project.Auth;
using AD_web_project.Data;
using AD_web_project.DTOs;
using AD_web_project.DTOs.GroupDTOs;
using AD_web_project.Models;
using AD_web_project.Services;
using AD_web_project.Services.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.EndPoints;

public static class GroupEndpoints
{

    public static void MapGroupEndpoints(this WebApplication app)
    {
        var groupCallGroup = app.MapGroup("/groups");



        // adding new group
        groupCallGroup.MapPost("/", async (
        CreateGroupDTO groupDTO,
        LocalContext dbcontext,
        HttpContext http,
        IAccessControlService controlService) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbcontext);
            var ruleActionType = ActionRule.RuleActionType.CreateGroup;
            var ruleTargetType = ActionRule.RuleTargetType.Group;
            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);

            if (!allowed)
                return ApiResponse.Fail($"User Is Unauthorized To Add Permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);

            if (string.IsNullOrWhiteSpace(groupDTO.GroupName))
                    return ApiResponse.Fail("Group Name can't be null or empty", 409);

            var normalized = groupDTO.GroupName.Trim().ToLower();
            bool exists = await dbcontext.Groups.AnyAsync(c => c.GroupName.Trim().ToLower() == normalized);
            if (exists)
                return ApiResponse.Fail($"Group ({groupDTO.GroupName}) Already Exsist", 409);
            

            Group newGroup = new()
            {
                GroupName = groupDTO.GroupName,
            
                
            };

            await dbcontext.Groups.AddAsync(newGroup);
            await dbcontext.SaveChangesAsync();

            //log the action 

            ActionLog action = new()
            {
                TargetGroupId = newGroup.Id,
                TargetGroup = newGroup,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.CreateGroup.ToString(),
                CreatedAt = DateTime.UtcNow

            };
            await dbcontext.ActionLogs.AddAsync(action);
            await dbcontext.SaveChangesAsync();
            return ApiResponse.Success($"Group ({groupDTO.GroupName}) Successfully Created", 201);


        }).RequireAuthorization();


        //Get all groups (cards)
        groupCallGroup.MapGet("/", async (LocalContext dbcontext) =>
        {
            var groups = await dbcontext.Groups
                .AsNoTracking()
                .Select(group => new GroupSummaryDTO(
                    group.Id,
                    group.GroupName,
                    group.GroupUsers.Count,
                    group.GroupComputers.Count,
                    group.GroupPermissions.Count
                ))
                .ToListAsync();

            return ApiResponse.Success("", groups);
        });

        // geting a group by Id(just the card)
        groupCallGroup.MapGet("/{id:int}", async (int id, LocalContext dbcontext) =>
        {
            var dto = await dbcontext.Groups
                .AsNoTracking()
                .Where(g => g.Id == id)
                .Select(g => new GroupSummaryDTO(
                    g.Id,
                    g.GroupName,
                    g.GroupUsers.Count,
                    g.GroupComputers.Count,
                    g.GroupPermissions.Count
                ))
                .FirstOrDefaultAsync();

            if (dto == null)
                return ApiResponse.Fail($"Group With {id} Not Found", 404);

            return ApiResponse.Success("", dto);
        });

        //get group by Name (just the card)
        groupCallGroup.MapGet("/by-name/{groupName}", async (string groupName, LocalContext dbcontext) =>
        {
            var normalized = groupName.Trim().ToLower();
            var dto = await dbcontext.Groups
                .AsNoTracking()
                .Where(g => g.GroupName.Trim().ToLower() == normalized)
                .Select(g => new GroupSummaryDTO(
                    g.Id,
                    g.GroupName,
                    g.GroupUsers.Count,
                    g.GroupComputers.Count,
                    g.GroupPermissions.Count
                ))
                .FirstOrDefaultAsync();

            if (dto == null)
                return ApiResponse.Fail($"Group '{groupName}' Not Found", 404);

            return ApiResponse.Success("", dto);
        });


        //get details group (when clicked)
        groupCallGroup.MapGet("/details-group/{groupId}", async (int groupId, LocalContext dbcontext) =>
        {
            var group = await dbcontext.Groups
                .AsNoTracking()
                .Where(g => g.Id == groupId)
                .Select(g => new GroupDetailsDTO(
                    g.Id,
                    g.GroupName,

                    g.GroupUsers
                        .Select(u => new UserSummaryDTO(
                            u.User.Id,
                            u.User.UserName
                        ))
                        .ToList(),

                    g.GroupComputers
                        .Select(c => new ComputerSummaryDTO(
                            c.Computer.Id,
                            c.Computer.ComputerName
                        ))
                        .ToList(),

                    g.GroupPermissions
                        .Select(p => p.Permission.PermissionName)
                        .ToList()

                ))
                .FirstOrDefaultAsync();

            if (group == null)
                return ApiResponse.Fail($"Group {groupId} Not Found", 404);

            return ApiResponse.Success("", group);
        });


        //get details group by name (when clicked)
        groupCallGroup.MapGet("/details-group-by-name/{groupName}", async (string groupName, LocalContext dbcontext) =>
        {
            var normalized = groupName.Trim().ToLower();
            var group = await dbcontext.Groups
                .AsNoTracking()
                .Where(g => g.GroupName.Trim().ToLower() == normalized)
                .Select(g => new GroupDetailsDTO(
                    g.Id,
                    g.GroupName,

                    g.GroupUsers
                        .Select(u => new UserSummaryDTO(
                            u.User.Id,
                            u.User.UserName
                        ))
                        .ToList(),

                    g.GroupComputers
                        .Select(c => new ComputerSummaryDTO(
                            c.Computer.Id,
                            c.Computer.ComputerName
                        ))
                        .ToList(),

                    g.GroupPermissions
                        .Select(p => p.Permission.PermissionName)
                        .ToList()

                ))
                .FirstOrDefaultAsync();

            if (group == null)
                return ApiResponse.Fail($"Group {groupName} Not Found", 404);

            return ApiResponse.Success("", group);
        });



        // add user or computer inside the user profile or group profile (id)
        groupCallGroup.MapPost("/add-user-group", async (
        AddToGroupUserDTO dto ,
        LocalContext dbcontext,
        IAccessControlService controlService , 
        HttpContext http) =>
        {

            var signedUser = await http.GetCurrentUserAsync(dbcontext);
            var ruleActionType = ActionRule.RuleActionType.AddToGroup;
            var ruleTargetType = ActionRule.RuleTargetType.User;

            var allowed = await controlService.CanAccessAsync(signedUser.Id,ruleActionType,ruleTargetType);
            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add user to group (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);

            var user = await dbcontext.Users.FindAsync(dto.UserId);
            if (user == null)
                return ApiResponse.Fail($"Failed To Find User With Id ({dto.UserId}) ,(Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);
            
            var group = await dbcontext.Groups.FirstOrDefaultAsync(p => p.Id== dto.GroupId);
            if (group == null)
                return ApiResponse.Fail($"Failed To Find Group with ID : {dto.GroupId}", 404);
            

            // if (group.Type != Group.GroupType.Users)
            //     return ApiResponse.Fail($"The User Cant ({user.UserName}) Cant Be Added to  ({group.GroupName})", 400);

            bool alreadyAssigned = await dbcontext.GroupUsers
            .AnyAsync(up => up.UserId== user.Id && up.GroupId == group.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"User ({user.UserName}) Alreday In ({group.GroupName}) ", 400);

            GroupUser newGroupUser = new()
            {
                UserId = user.Id,
                GroupId = group.Id
            };
            await dbcontext.GroupUsers.AddAsync(newGroupUser);
            await dbcontext.SaveChangesAsync(); 

            for (int i =0 ;i<group.GroupPermissions.Count; i++)
            {

              Console.Write(group.GroupPermissions[i].ToString()); 
             
            };
         

            //Log the Action

            ActionLog log = new()
            {
                TargetUserId =user.Id,
                TargetUser = user,
                TargetGroupId= group.Id,
                TargetGroup =group,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.AddToGroup.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync(); 
        

            return ApiResponse.Success($"User {user.UserName} Successfully Add to Group {group.GroupName}",200); 
            
            
        }).RequireAuthorization();


        // add computer inside the user profile or group profile (id)
        groupCallGroup.MapPost("/add-computer-group", async (
        AddToGroupComputerDTO dto ,
        LocalContext dbcontext,
        IAccessControlService controlService , 
        HttpContext http) =>
        {

            var signedUser = await http.GetCurrentUserAsync(dbcontext);
            var ruleActionType = ActionRule.RuleActionType.AddToGroup;
            var ruleTargetType = ActionRule.RuleTargetType.Computer;

            var allowed = await controlService.CanAccessAsync(signedUser.Id,ruleActionType,ruleTargetType);
            if (!allowed)
                return ApiResponse.Fail($"User is unauthorized to add computer (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);

            var computer = await dbcontext.Computers.FindAsync(dto.ComputerId);
            if (computer == null)
                return ApiResponse.Fail($"Failed To Find Computer With Id ({dto.ComputerId}) ,(Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);
            
            var group = await dbcontext.Groups.FirstOrDefaultAsync(p => p.Id== dto.GroupId);
            if (group == null)
                return ApiResponse.Fail($"Failed To Find Group with ID : {dto.GroupId}", 404);
            

            // if (group.Type != Group.GroupType.Computer)
            //     return ApiResponse.Fail($"The Computer Cant ({computer.ComputerName}) Cant Be Added to  ({group.GroupName})", 400);

            bool alreadyAssigned = await dbcontext.GroupComputers
            .AnyAsync(up => up.ComputerId== computer.Id && up.GroupId == group.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"Computer ({computer.ComputerName}) Alreday In ({group.GroupName}) ", 400);

            GroupComputer newGroupComputer = new()
            {
                ComputerId = computer.Id,
                GroupId = group.Id
            };
            await dbcontext.GroupComputers.AddAsync(newGroupComputer);
            await dbcontext.SaveChangesAsync(); 

            //Log the Action

            ActionLog log = new()
            {
                TargetComputerId =computer.Id,
                TargetComputer = computer,
                TargetGroupId= group.Id,
                TargetGroup =group,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.AddToGroup.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync(); 

            return ApiResponse.Success($"Computer {computer.ComputerName} Successfully Add to Group {group.GroupName}",200); 
            
            
        }).RequireAuthorization();


    }


    
    }
    




