using System.ComponentModel.DataAnnotations;
using static IAM_API.Models.ActionRule;

namespace IAM_API.DTOs.RulesDTOs;

public record  CreateActionRuleDTO
(
    [Required] RuleActionType ActionType , 
    [Required] RuleTargetType TargetType, 
    [Required] string RequiredPermissionName 
);
