using System;
using AD_web_project.Data;
using AD_web_project.Models;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.Services.AccessControl;

public class AccessControlService : IAccessControlService
{
    private readonly LocalContext dbcontext; 
    public AccessControlService(LocalContext dbcontext)
    {
        this.dbcontext = dbcontext; 
    }
    public async Task<bool> CanAccessAsync(int userId, ActionRule.RuleActionType actionType, ActionRule.RuleTargetType targetType )
    {
        var user = await dbcontext.Users
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;
        
        var rule = await dbcontext.ActionRules.Where(r=> r.ActionType== actionType && r.TargetType== targetType)
        .ToListAsync(); 

        if (rule.Count == 0)
            return false ;
        
        var userPermissionIds = user.UserPermissions
            .Select(p => p.PermissionId)
            .ToList();

  
        var allowed = rule.Any(r =>
            userPermissionIds.Contains(r.RequiredPermissionId));
        

        return allowed;
            
        
    }
}
