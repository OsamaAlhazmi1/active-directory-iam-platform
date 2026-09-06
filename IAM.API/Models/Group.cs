using System;

namespace IAM_API.Models;

public class Group
{


    public int Id { get; set; }

    public string GroupName { get; set; } = "";


    // RELATIONS
    public List<GroupUser> GroupUsers { get; set; } = new();
    public List<GroupComputer> GroupComputers { get; set; } = new();
    public List<GroupPermission> GroupPermissions { get; set; } = new();


}
