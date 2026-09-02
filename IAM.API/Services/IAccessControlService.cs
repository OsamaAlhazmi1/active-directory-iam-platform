using System;
using AD_web_project.Models;

namespace AD_web_project.Services.AccessControl;

public interface IAccessControlService
{
   Task<bool> CanAccessAsync(int userId, ActionRule.RuleActionType actionType, ActionRule.RuleTargetType targetType);
   
}
