using System.ComponentModel.DataAnnotations;
using IAM_API.Models;

namespace IAM_API.DTOs;

public record  UserDetailsDTO
(
 [Required] int UserId ,
 [Required] string UserName, 
 [Required] string [] UserPermissions ,
 [Required] string [] UserGroups

);
   


