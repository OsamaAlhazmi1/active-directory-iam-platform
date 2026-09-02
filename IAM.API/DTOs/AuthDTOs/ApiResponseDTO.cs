using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs.AuthDTOs;


public record ApiResponseDTO<T>(
   bool Success,
   string Message,
   T Data
);