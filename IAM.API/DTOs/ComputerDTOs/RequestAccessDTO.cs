using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  RequestAccessDTO(

    [Required] string ComputerName


);

