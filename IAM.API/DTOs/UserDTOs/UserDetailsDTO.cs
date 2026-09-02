using System.ComponentModel.DataAnnotations;
using AD_web_project.Models;

namespace AD_web_project.DTOs;

public record  UserDetailsDTO
(
 [Required] int UserId ,
 [Required] string UserName, 
 [Required] string [] UserPermissions ,
 [Required] string [] UserGroups

);
   


