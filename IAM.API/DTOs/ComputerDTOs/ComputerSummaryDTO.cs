using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  ComputerSummaryDTO(

    [Required] int ComputerID,
    [Required] string ComputerName
    


);

