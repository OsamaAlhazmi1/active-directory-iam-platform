using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.GroupDTOs;

public record  GroupSummaryDTO
(
    int Id,
    string GroupName,
    int UsersCount,
    int ComputersCount,
    int PermissionsCount
   
);


