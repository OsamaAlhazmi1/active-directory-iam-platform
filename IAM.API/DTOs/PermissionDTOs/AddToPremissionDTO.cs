using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  AddToPremissionDTO
(
    [Required] string PermissionName

);
