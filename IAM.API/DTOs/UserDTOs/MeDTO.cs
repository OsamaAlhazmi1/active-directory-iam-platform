using System.ComponentModel.DataAnnotations;

namespace IAM_API.DTOs;

public record MeDto(
    int UserId,
    string UserName,
    List<string> Permissions,
    List<string> Groups
);
