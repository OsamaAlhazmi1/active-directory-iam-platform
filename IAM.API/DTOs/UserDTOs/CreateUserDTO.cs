using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  CreateUserDTO
(
    [Required] string UserName, 
    [Required] string Password

);
