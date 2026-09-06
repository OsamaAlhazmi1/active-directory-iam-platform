using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  ComputerSummaryDTO(

    [Required] int ComputerID,
    [Required] string ComputerName
    


);

