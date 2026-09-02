using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs.GroupDTOs;

public record  GroupSummaryDTO
(
    int Id,
    string GroupName,
    int UsersCount,
    int ComputersCount,
    int PermissionsCount
   
);


