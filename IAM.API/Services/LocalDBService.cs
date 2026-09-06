using System;
using System.DirectoryServices.Protocols;
using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using IAM_API.DTOs.AD_DTOs;
using IAM_API.Services.AccessControl;
using IAM_API.Models;
using IAM_API.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_API.Services;

public class LocalDBService : ILocalDBService
{
//    private readonly LocalContext dbcontext; 

//    public LocalDBService(LocalContext dbcontext)
//     {
//         this.dbcontext = dbcontext; 
//     }

//     public async Task<Permission?> GetPermissionByNameAsync(string permissionName)
//     {
//        var permission = await dbcontext.Permissions.FirstOrDefaultAsync(p=>p.PermissionName == permissionName);
//        return permission;
//     }

//     public async Task<User?> GetUserByIdAsync(int userId)
//     {
//         var user = await dbcontext.Users.FirstOrDefaultAsync(u=>u.Id==userId); 
//         return user;
//     }
}