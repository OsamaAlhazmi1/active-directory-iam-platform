namespace AD_web_project.DTOs.AD_DTOs;

public  record ADUserDetailsDTO
(
    string SamAccountName,
    string DisplayName,
    string DistinguishedName,
    string Email,
    string EmployeID,
    string [] MemberOf, 
    bool Enabled , 
    bool LockedOut


);

