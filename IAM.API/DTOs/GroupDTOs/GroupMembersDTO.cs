using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs.GroupDTOs;

public record  GroupMembersDTO
(
    string GroupName,
    int UsersCount,
    int ComputersCount,
    int PermissionsCount
   
);


