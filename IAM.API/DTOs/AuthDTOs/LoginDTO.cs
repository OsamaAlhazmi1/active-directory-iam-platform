using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs.AuthDTOs;

public record  LoginDTO
(
    [Required] string UserName,
    [Required] string Password
);


