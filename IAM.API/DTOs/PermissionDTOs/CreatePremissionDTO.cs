using System.ComponentModel.DataAnnotations;
using static IAM_API.Models.Permission;

namespace IAM_API.DTOs;

public record  CreatePremissionDTO
(
    [Required] string PermissionName,
    [Required]  PermissionType Permission_Type

);
