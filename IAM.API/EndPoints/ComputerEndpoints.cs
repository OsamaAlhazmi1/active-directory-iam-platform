using System;
using AD_web_project.Auth;
using AD_web_project.Data;
using AD_web_project.DTOs;
using AD_web_project.Models;
using AD_web_project.Services;
using AD_web_project.Services.AccessControl;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.EndPoints;

public static class ComputerEndpoints
{
    const string GetAllLocalCompuertsEndpointName = "GetLocalComputer";


    public static void MapComputerEndpoints(this WebApplication app)
    {

        var computersGroup = app.MapGroup("/computers");



        /// local Computers Endpoints (crads)
        computersGroup.MapGet("/", async (LocalContext dbcontext) =>
        {
            var allLocalComputers = await dbcontext.Computers.Select(computers => new ComputerSummaryDTO(
                computers.Id, computers.ComputerName
            )).AsNoTracking().ToListAsync();

            return ApiResponse.Success("", allLocalComputers);

        }).WithName(GetAllLocalCompuertsEndpointName);


        //get computer by id (just card)
        computersGroup.MapGet("/{id:int}", async (int id, LocalContext dbcontext) =>
        {
            var computer = await dbcontext.Computers
                .FirstOrDefaultAsync(u => u.Id == id);

            if (computer == null)
                return ApiResponse.Fail($"Computer With {id} Not Found", 404);

            var dto = new ComputerSummaryDTO (
                computer.Id,
                computer.ComputerName
            );

            return ApiResponse.Success("", dto);
        });

        //get Computer by Name(just card)
        computersGroup.MapGet("/by-name/{computerName}", async (string CoumputerName, LocalContext dbcontext) =>
        {
            var computer = await dbcontext.Computers
                .FirstOrDefaultAsync(u => u.ComputerName == CoumputerName);

            if (computer == null)
                return ApiResponse.Fail($"Computer With {CoumputerName} Not Found", 404);

            var dto = new ComputerSummaryDTO(
                computer.Id,
                computer.ComputerName
            );

            return ApiResponse.Success("", dto);
        });


        // get details of Computer (Computer name + Computer id + all of permesions)
        computersGroup.MapGet("/details-Computer/{id}", async (int id, LocalContext dbcontext) =>
        {
            var computer = await dbcontext.Computers
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new ComputerDetailsDTO(
                    u.Id,
                    u.ComputerName,

                    // PERMISSIONS
                    u.ComputerPermissions
                        .Select(up => up.Permission.PermissionName)
                        .ToArray(),

                    // GROUPS 
                    u.GroupComputers
                        .Select(g => g.Group.GroupName)
                        .ToArray()
                ))
                .FirstOrDefaultAsync();

            if (computer == null)
                return ApiResponse.Fail($"Computer With id {id} Not Found", 404);

            return ApiResponse.Success("", computer);
        });





        // creating new Computer+ ComuterGroup
        computersGroup.MapPost("/", async (
            CreateComputersDTO dto,
            LocalContext dbcontext,
            HttpContext http,
            IAccessControlService controlService) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbcontext);
            var ruleActionType = ActionRule.RuleActionType.CreateComputer;
            var ruleTargetType = ActionRule.RuleTargetType.Computer;

            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);
            if (!allowed)
                return ApiResponse.Fail($"User Is Unauthorized To Add Permission (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);

            if (string.IsNullOrWhiteSpace(dto.ComputerName))
                return ApiResponse.Fail("Computer Name can't be null or empty", 409);

            var normalized = dto.ComputerName.Trim().ToLower();

            bool exists = await dbcontext.Computers.AnyAsync(c => c.ComputerName.Trim().ToLower() == normalized);
            if (exists)
                return ApiResponse.Fail($"Computer ({dto.ComputerName}) Already Exsist", 409);

            Computer newComputer = new()
            {
                ComputerName = dto.ComputerName
            };

            await dbcontext.Computers.AddAsync(newComputer);
            await dbcontext.SaveChangesAsync();

            // creating new computer group

            Group newGroup = new()
            {
                GroupName = dto.ComputerName + "-A",

            };


            //log the create computer action 

            ActionLog action = new()
            {

                TargetComputerId = newComputer.Id,
                TargetComputer = newComputer,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.CreateComputer.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(action);
            await dbcontext.SaveChangesAsync();


            //log the create group 

            ActionLog action_2 = new()
            {

                TargetGroupId = newGroup.Id,
                TargetGroup = newGroup,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.CreateGroup.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(action_2);
            await dbcontext.SaveChangesAsync();


            return ApiResponse.Success($"Computer ({dto.ComputerName}) Successfully Created", 201);


        }).RequireAuthorization();



        // Granting a singend User access to Server 
        computersGroup.MapPost("/request-access", async (
            RequestAccessDTO dto,
            LocalContext dbcontext,
            HttpContext http,
            IAccessControlService controlService) =>
        {
            var signedUser = await http.GetCurrentUserAsync(dbcontext);
            var ruleActionType = ActionRule.RuleActionType.Access;
            var ruleTargetType = ActionRule.RuleTargetType.Computer;


            var allowed = await controlService.CanAccessAsync(signedUser.Id, ruleActionType, ruleTargetType);
            if (!allowed)
                return ApiResponse.Fail($"User is Unauthorized To Request Access  (Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 401);

            var computer = await dbcontext.Computers
            .FirstOrDefaultAsync(c => c.ComputerName == dto.ComputerName);
            if (computer == null)
                return ApiResponse.Fail($"Failed To Find Computer With Name ({dto.ComputerName}) ,(Action = {ruleActionType.ToString()} , Taget Type = {ruleTargetType.ToString()})", 404);

            var group = await dbcontext.Groups.FirstOrDefaultAsync(p => p.GroupName == dto.ComputerName + "-A");
            if (group == null)
                return ApiResponse.Fail($"Failed To Find Server with Name : {dto.ComputerName}", 404);

            bool alreadyAssigned = await dbcontext.GroupUsers
            .AnyAsync(up => up.UserId == signedUser.Id && up.GroupId == group.Id);
            if (alreadyAssigned)
                return ApiResponse.Fail($"User ({signedUser.UserName}) Alreday Have Access To ({group.GroupName}) ", 400);

            GroupUser newGroupUser = new()
            {
                UserId = signedUser.Id,
                GroupId = group.Id
            };
            await dbcontext.GroupUsers.AddAsync(newGroupUser);
            await dbcontext.SaveChangesAsync();

            //Log the Action

            ActionLog log = new()
            {
                TargetUserId = signedUser.Id,
                TargetUser = signedUser,
                TargetGroupId = group.Id,
                TargetGroup = group,
                ActionCreatedByUserId = signedUser.Id,
                ActionCreatedByUser = signedUser,
                Action_Type = ActionRule.RuleActionType.Access.ToString(),
                CreatedAt = DateTime.Now

            };
            await dbcontext.ActionLogs.AddAsync(log);
            await dbcontext.SaveChangesAsync();

            return ApiResponse.Success($"User {signedUser.UserName} Successfully Granted Access {dto.ComputerName}", 200);

        }).RequireAuthorization();


    }

}
