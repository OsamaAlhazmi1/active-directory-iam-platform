using System;
using AD_web_project.Data;
using AD_web_project.Models;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.Auth;

public static class HttpContextExtensions
{
        public static async Task<User?> GetCurrentUserAsync(
        this HttpContext http,
        LocalContext dbcontext)
    {
        var idClaim = http.User.FindFirst("id")?.Value;
        if (idClaim is null) return null;

        var userId = int.Parse(idClaim);

        return await dbcontext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}
