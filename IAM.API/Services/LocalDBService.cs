using System;
using System.DirectoryServices.Protocols;
using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using AD_web_project.DTOs.AD_DTOs;
using AD_web_project.Services.AccessControl;
using AD_web_project.Models;
using AD_web_project.Data;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.Services;

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