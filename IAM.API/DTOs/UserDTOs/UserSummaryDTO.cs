using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  UserSummaryDTO

(
    [Required] int UserId , 
    [Required]string UserName
);
