using System;

namespace AD_web_project.Models;

public class User
{
    public int Id {get; set;}
    public string UserName {get; set;}=""; 
    public List<UserPermission> UserPermissions { get; set; } = new();
    public List<GroupUser> GroupUsers { get; set; } = new();

    public string Password { get; set; } = "";

}
