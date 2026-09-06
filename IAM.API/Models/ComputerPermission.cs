using System;

namespace IAM_API.Models;

public class ComputerPermission
{
    public int Id { get; set; }

    public int ComputerId { get; set; }
    public Computer Computer { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
