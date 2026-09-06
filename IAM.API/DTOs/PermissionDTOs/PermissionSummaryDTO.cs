using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.PermissionDTOs;

public record  PermissionSummaryDTO
(
    [Required] int PermissionID ,
    [Required] string PermissionName,
    string Permission_Type
);
