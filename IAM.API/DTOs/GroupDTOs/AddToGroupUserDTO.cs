using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.GroupDTOs;

public record  AddToGroupUserDTO
(

    int UserId, 
    int GroupId
   
);


