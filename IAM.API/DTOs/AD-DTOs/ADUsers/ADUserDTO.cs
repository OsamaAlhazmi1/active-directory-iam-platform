namespace IAM_API.DTOs.AD_DTOs;

public  record ADUserDTO
(
    string SamAccountName,
    string DisplayName,
    string DistinguishedName,
    string Email, 
    bool Enabled

);

