using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  CreateComputersDTO
(
    [Required] string ComputerName
);