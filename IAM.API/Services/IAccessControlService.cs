using System;
using IAM_API.Models;

namespace IAM_API.Services.AccessControl;

public interface IAccessControlService
{
   Task<bool> CanAccessAsync(int userId, ActionRule.RuleActionType actionType, ActionRule.RuleTargetType targetType);
   
}
