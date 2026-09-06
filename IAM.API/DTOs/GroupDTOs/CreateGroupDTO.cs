using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static IAM_API.Models.Group;

namespace IAM_API.DTOs.GroupDTOs;

public record  CreateGroupDTO
(
    [Required] string GroupName
   
);


