using System;

namespace IAM_API.Models;

public class Permission
{
    public enum PermissionType
    {
        User,
        Computer,
        Group
    } 


    public int Id { get; set; }
    public string PermissionName { get; set; } = "";

    public PermissionType Permission_Type {get; set;}

}
