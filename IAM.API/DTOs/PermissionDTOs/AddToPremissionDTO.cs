using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  AddToPremissionDTO
(
    [Required] string PermissionName

);
