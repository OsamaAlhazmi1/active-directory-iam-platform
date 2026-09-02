using System;

namespace AD_web_project.Models;

public class GroupPermission
{
    public int Id { get; set; }
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

}