using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  ComputerDetailsDTO
(
 [Required] int ComputerId ,
 [Required] string ComputerName, 
 [Required] string [] ComputerPremssions,
 [Required] string [] ComputerGroups
    
);
   


