namespace IAM.UI.DTOs;

public record MeDTO(
    int UserId,
    string UserName,
    List<string> Permissions
);