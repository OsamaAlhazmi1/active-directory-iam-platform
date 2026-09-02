using System;

namespace AD_web_project.Models;

public class ActionRule
{
    public enum RuleActionType
    {
        CreateComputer,
        RemoveComputer,
        UpdateComputer,
        CreatePermission,
        RemovePermission,
        UpdatePermission,
        AssignPermission,
        CreateGroup,
        RemoveGroup,
        UpdateGroup,
        AddToGroup,
        RemoveFromGroup,
        Access
    }

    public enum RuleTargetType
    {
        User,
        Computer,
        Permission,
        Group
    }

    public int Id { get; set; }

    public RuleActionType ActionType { get; set; }

    public RuleTargetType TargetType { get; set; }

    public int RequiredPermissionId { get; set; }

    public Permission RequiredPermission { get; set; } = null!;

}
