using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.GroupDTOs;

public record  GroupMembersDTO
(
    string GroupName,
    int UsersCount,
    int ComputersCount,
    int PermissionsCount
   
);


