using System.ComponentModel.DataAnnotations;
using AD_web_project.Models;

namespace AD_web_project.DTOs;

public record  ActionLogsDetailsDTO
(
    [Required] int Id,
    int? TragetUserId, 
    string? TragetUserName,
    int? TragetCompterId, 
    string? TragetCompterName,
    int? TargetPermissionId, 
    string? TargetPermissionName,
    int? TargetGroupId, 
    string? TargetGroupName,
    [Required] int ActionCreatedByUserId, 
    [Required] string ActionCreatedByUserName,
    [Required] string Action_Type, 
    [Required] DateTime CreatedAt


);