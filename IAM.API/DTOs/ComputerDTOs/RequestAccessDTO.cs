using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record  RequestAccessDTO(

    [Required] string ComputerName


);

