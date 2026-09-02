using System.ComponentModel.DataAnnotations;
using static AD_web_project.Models.ActionRule;

namespace AD_web_project.DTOs.RulesDTOs;

public record  CreateActionRuleDTO
(
    [Required] RuleActionType ActionType , 
    [Required] RuleTargetType TargetType, 
    [Required] string RequiredPermissionName 
);
