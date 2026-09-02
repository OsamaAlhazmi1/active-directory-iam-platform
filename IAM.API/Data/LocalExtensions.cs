using System;
using AD_web_project.Models;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.Data;

public static class LocalExtensions
{
    public static void AddLocalDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("ADLocalDBKey");
        builder.Services.AddSqlite<LocalContext>(
            connString,
            optionsAction: option => option.UseSeeding((dbcontext, _) =>
            {
                SeedPermissions((LocalContext)dbcontext);
                SeedIamUser((LocalContext)dbcontext);
                SeedUsersGroupsAndComputers((LocalContext)dbcontext);
                SeedRules((LocalContext)dbcontext);
            })

);

    }


    private static void SeedPermissions(LocalContext dbcontext)
    {
        if (dbcontext.Set<Permission>().Any())
            return;

        dbcontext.Set<Permission>().AddRange(

            new Permission { PermissionName = "IAM Admin", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Domain Admin", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Account Operator", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Helpdesk Operator", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Password Reset", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Unlock User", Permission_Type = Permission.PermissionType.User },

            new Permission { PermissionName = "Server Access", Permission_Type = Permission.PermissionType.Group },

            new Permission { PermissionName = "VPN Access", Permission_Type = Permission.PermissionType.Group },

            new Permission { PermissionName = "HR System Access", Permission_Type = Permission.PermissionType.Group },

            new Permission { PermissionName = "Finance System Access", Permission_Type = Permission.PermissionType.Group },

            new Permission { PermissionName = "Domain Computer", Permission_Type = Permission.PermissionType.Computer },

            new Permission { PermissionName = "Privileged Workstation", Permission_Type = Permission.PermissionType.Computer }

        );

        dbcontext.SaveChanges();
    }
    private static void SeedIamUser(LocalContext context)
    {
        var iamPermission = context.Set<Permission>()
            .FirstOrDefault(p => p.PermissionName == "IAM Admin");

        if (iamPermission == null)
            return;

        if (context.Set<User>().Any())
            return;

        var iamUser = new User
        {
            UserName = "IAM Admin",
            Password = "123456789"


        };

        context.Set<User>().Add(iamUser);
        context.SaveChanges();

        context.Set<UserPermission>().Add(
            new UserPermission
            {
                UserId = iamUser.Id,
                PermissionId = iamPermission.Id
            }
        );

        context.SaveChanges();
    }
    private static void SeedUsersGroupsAndComputers(LocalContext context)
    {
        if (context.Set<User>().Count() > 1)
            return;

        // ================= USERS =================

        var users = new List<User>
    {
        new() { UserName = "john.smith", Password = "123456789" },
        new() { UserName = "sarah.connor", Password = "123456789" },
        new() { UserName = "michael.brown", Password = "123456789" },
        new() { UserName = "emma.wilson", Password = "123456789" },
        new() { UserName = "david.lee", Password = "123456789" },
        new() { UserName = "alex.johnson", Password = "123456789" },
        new() { UserName = "james.anderson", Password = "123456789" },
        new() { UserName = "olivia.martin", Password = "123456789" },
        new() { UserName = "daniel.white", Password = "123456789" },
        new() { UserName = "ethan.clark", Password = "123456789" },

        new() { UserName = "nora.evans", Password = "123456789" },
        new() { UserName = "linda.scott", Password = "123456789" },
        new() { UserName = "victor.king", Password = "123456789" },
        new() { UserName = "ryan.hill", Password = "123456789" },
        new() { UserName = "grace.walker", Password = "123456789" },
        new() { UserName = "henry.young", Password = "123456789" },
        new() { UserName = "leo.green", Password = "123456789" },
        new() { UserName = "sophia.adams", Password = "123456789" },
        new() { UserName = "adam.turner", Password = "123456789" },
        new() { UserName = "mia.carter", Password = "123456789" }
    };

        context.Users.AddRange(users);

        // ================= COMPUTERS =================

        var computers = new List<Computer>
    {
        new() { ComputerName = "DC-01" },
        new() { ComputerName = "DC-02" },

        new() { ComputerName = "FS-01" },
        new() { ComputerName = "FS-02" },

        new() { ComputerName = "APP-01" },
        new() { ComputerName = "APP-02" },

        new() { ComputerName = "SQL-01" },
        new() { ComputerName = "SQL-02" },

        new() { ComputerName = "WEB-01" },
        new() { ComputerName = "WEB-02" },

        new() { ComputerName = "HR-PC-01" },
        new() { ComputerName = "HR-PC-02" },

        new() { ComputerName = "FIN-PC-01" },
        new() { ComputerName = "FIN-PC-02" },

        new() { ComputerName = "ADM-WS-01" },
        new() { ComputerName = "ADM-WS-02" },

        new() { ComputerName = "HELPDESK-01" },
        new() { ComputerName = "HELPDESK-02" },

        new() { ComputerName = "VPN-GW-01" },
        new() { ComputerName = "MGMT-SRV-01" }
    };

        context.Computers.AddRange(computers);

        // ================= GROUPS =================

        var groups = new List<Group>
    {
        // Department / Role Groups
        new() { GroupName = "Domain Admins" },
        new() { GroupName = "Account Operators" },
        new() { GroupName = "Helpdesk Team" },
        new() { GroupName = "HR Team" },
        new() { GroupName = "Finance Team" },
        new() { GroupName = "VPN Users" },

        // Server Access Groups (-A)
        new() { GroupName = "DC-01-A" },
        new() { GroupName = "DC-02-A" },

        new() { GroupName = "FS-01-A" },
        new() { GroupName = "FS-02-A" },

        new() { GroupName = "APP-01-A" },
        new() { GroupName = "APP-02-A" },

        new() { GroupName = "SQL-01-A" },
        new() { GroupName = "SQL-02-A" },

        new() { GroupName = "WEB-01-A" },
        new() { GroupName = "WEB-02-A" },

        new() { GroupName = "VPN-GW-01-A" },
        new() { GroupName = "MGMT-SRV-01-A" }
    };
    

        context.Groups.AddRange(groups);
        

        context.SaveChanges();
    }

    private static void SeedRules(LocalContext context)
    {
        if (context.Set<ActionRule>().Any())
            return;

        var iamPermission = context.Set<Permission>()
            .FirstOrDefault(p => p.PermissionName == "IAM Admin");

        if (iamPermission == null)
            return;

        context.Set<ActionRule>().AddRange(
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.CreatePermission,
                TargetType = ActionRule.RuleTargetType.Permission,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.CreateComputer,
                TargetType = ActionRule.RuleTargetType.Computer,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.AssignPermission,
                TargetType = ActionRule.RuleTargetType.User,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.AssignPermission,
                TargetType = ActionRule.RuleTargetType.Computer,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.AssignPermission,
                TargetType = ActionRule.RuleTargetType.Group,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.CreateGroup,
                TargetType = ActionRule.RuleTargetType.Group,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.AddToGroup,
                TargetType = ActionRule.RuleTargetType.User,
                RequiredPermissionId = iamPermission.Id
            },
            new ActionRule
            {
                ActionType = ActionRule.RuleActionType.AddToGroup,
                TargetType = ActionRule.RuleTargetType.Computer,
                RequiredPermissionId = iamPermission.Id
            },
             new ActionRule
             {
                 ActionType = ActionRule.RuleActionType.Access,
                 TargetType = ActionRule.RuleTargetType.User,
                 RequiredPermissionId = iamPermission.Id
             }


        );

        context.SaveChanges();
    }


    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LocalContext>();
        db.Database.Migrate();
    }
}
