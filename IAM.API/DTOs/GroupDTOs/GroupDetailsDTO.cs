using System.ComponentModel.DataAnnotations;
using IAM_API.DTOs.PermissionDTOs;

namespace IAM_API.DTOs.GroupDTOs;

public record GroupDetailsDTO
(
    [Required] int Id,
    [Required] string GroupName,
    List<UserSummaryDTO> GroupUsers,
    List<ComputerSummaryDTO> GroupComputers,
    List<string> GroupPermissions
   



);
