using System;
using IAM_API.Data;
using IAM_API.DTOs;
using IAM_API.Services;
using Microsoft.EntityFrameworkCore;

namespace IAM_API.EndPoints;

public static class ActionLogsEndpoints
{
    const string GetAllLocalActionEndpointName = "GetActionLogs";


    public static void MapActionLogsEndpoints(this WebApplication app)
    {

        var actionLogGroup = app.MapGroup("/actionLogs");

        actionLogGroup.MapGet("", async (LocalContext dbcontext) =>
        {
            var allActionLogs = await dbcontext.ActionLogs
                .Include(l => l.TargetUser)
                .Include(l => l.TargetComputer)
                .Include(l => l.TargetPermission)
                .Include(l => l.ActionCreatedByUser)
                .AsNoTracking()
                .Select(log => new ActionLogsDetailsDTO(
                    log.Id,
                    log.TargetUserId,
                    log.TargetUser != null ? log.TargetUser.UserName : null,
                    log.TargetComputerId,
                    log.TargetComputer != null ? log.TargetComputer.ComputerName : null,
                    log.TargetPermissionId,
                    log.TargetPermission != null ? log.TargetPermission.PermissionName : null,
                    log.TargetGroupId,
                    log.TargetGroup != null ? log.TargetGroup.GroupName : null,
                    log.ActionCreatedByUserId,
                    log.ActionCreatedByUser.UserName,
                    log.Action_Type,
                    log.CreatedAt
                ))
                .ToListAsync();

            return ApiResponse.Success("",allActionLogs);

        }).WithName(GetAllLocalActionEndpointName);



    }

}
