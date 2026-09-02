using System.ComponentModel.DataAnnotations;
using static AD_web_project.Models.ActionRule;

namespace AD_web_project.DTOs.RulesDTOs;

public record  ActionRuleDetailsDTO
(
    [Required] int RuleID,
    [Required] string ActionType , 
    [Required] string TargetType, 
    [Required] string RequiredPermissionName 
);
