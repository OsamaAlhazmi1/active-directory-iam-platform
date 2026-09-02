using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  CreateUserDTO
(
    [Required] string UserName, 
    [Required] string Password

);
