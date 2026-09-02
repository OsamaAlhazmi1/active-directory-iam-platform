using System;

namespace AD_web_project.Models;

public class ActionLog
{


    public int Id { get; set; }

    public int? TargetUserId { get; set; }
    public User? TargetUser { get; set; }

    public int? TargetComputerId { get; set; }
    public Computer? TargetComputer { get; set; }

    public int? TargetPermissionId { get; set; }
    public Permission? TargetPermission { get; set; }
    public int? TargetGroupId { get; set; }
    public Group? TargetGroup { get; set; }

    public int ActionCreatedByUserId { get; set; }
    public User ActionCreatedByUser { get; set; } = null!;

    public string Action_Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
