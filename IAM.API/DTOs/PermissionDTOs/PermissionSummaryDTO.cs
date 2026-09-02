using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs.PermissionDTOs;

public record  PermissionSummaryDTO
(
    [Required] int PermissionID ,
    [Required] string PermissionName,
    string Permission_Type
);
