namespace IAM.UI.DTOs;

public record ApiResponseDTO<T>(
    bool Success,
    string Message,
    T Data
);