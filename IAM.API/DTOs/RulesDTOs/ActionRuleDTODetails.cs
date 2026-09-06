using System.ComponentModel.DataAnnotations;
using static IAM_API.Models.ActionRule;

namespace IAM_API.DTOs.RulesDTOs;

public record  ActionRuleDetailsDTO
(
    [Required] int RuleID,
    [Required] string ActionType , 
    [Required] string TargetType, 
    [Required] string RequiredPermissionName 
);
