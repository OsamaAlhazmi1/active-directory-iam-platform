using System;
using AD_web_project.DTOs.AD_DTOs;

namespace AD_web_project.Services;

public interface IActiveDirectoryService
{
    //Task<bool> UserExistsAsync(string username);
    Task<ADUserDetailsDTO> GetUserDetailsAsync(string username);
    //Task<bool> AddUserToGroupAsync(string username, string groupName);
    Task<bool> DisableUserAsync(string username);
    Task<bool> EnableUserAsync(string username);
    Task<List<ADUserDTO>> GetUsersAsync();
    Task<bool> ResetPasswordAsync(string username, string newPassword);

}
