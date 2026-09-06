using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record  ComputerDetailsDTO
(
 [Required] int ComputerId ,
 [Required] string ComputerName, 
 [Required] string [] ComputerPremssions,
 [Required] string [] ComputerGroups
    
);
   


