using System.ComponentModel.DataAnnotations;
using AD_web_project.DTOs.PermissionDTOs;

namespace AD_web_project.DTOs.GroupDTOs;

public record GroupDetailsDTO
(
    [Required] int Id,
    [Required] string GroupName,
    List<UserSummaryDTO> GroupUsers,
    List<ComputerSummaryDTO> GroupComputers,
    List<string> GroupPermissions
   



);
