using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs.AuthDTOs;


public record ApiResponseDTO<T>(
   bool Success,
   string Message,
   T Data
);