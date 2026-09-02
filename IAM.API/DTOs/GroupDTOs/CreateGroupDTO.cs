using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static AD_web_project.Models.Group;

namespace AD_web_project.DTOs.GroupDTOs;

public record  CreateGroupDTO
(
    [Required] string GroupName
   
);


