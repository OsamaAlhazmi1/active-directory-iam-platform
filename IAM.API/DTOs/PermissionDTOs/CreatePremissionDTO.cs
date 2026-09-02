using System.ComponentModel.DataAnnotations;
using static AD_web_project.Models.Permission;

namespace AD_web_project.DTOs;

public record  CreatePremissionDTO
(
    [Required] string PermissionName,
    [Required]  PermissionType Permission_Type

);
