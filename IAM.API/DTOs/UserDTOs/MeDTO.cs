using System.ComponentModel.DataAnnotations;

namespace AD_web_project.DTOs;

public record MeDto(
    int UserId,
    string UserName,
    List<string> Permissions,
    List<string> Groups
);
