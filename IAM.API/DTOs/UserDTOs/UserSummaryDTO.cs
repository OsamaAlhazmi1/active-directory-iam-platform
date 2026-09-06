using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  UserSummaryDTO

(
    [Required] int UserId , 
    [Required]string UserName
);
