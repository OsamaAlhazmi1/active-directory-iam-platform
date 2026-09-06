using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.AuthDTOs;

public record  LoginDTO
(
    [Required] string UserName,
    [Required] string Password
);


