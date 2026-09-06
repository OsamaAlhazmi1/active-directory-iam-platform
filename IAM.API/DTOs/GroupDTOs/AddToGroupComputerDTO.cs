using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.GroupDTOs;

public record  AddToGroupComputerDTO
(

    int ComputerId, 
    int GroupId
   
);


