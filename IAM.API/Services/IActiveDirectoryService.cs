using System;
using IAM_API.DTOs.AD_DTOs;

namespace IAM_API.Services;

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
