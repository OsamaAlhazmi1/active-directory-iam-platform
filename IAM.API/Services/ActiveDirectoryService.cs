using System;
using System.DirectoryServices.Protocols;
using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using AD_web_project.DTOs.AD_DTOs;

namespace AD_web_project.Services;

public class ActiveDirectoryService : IActiveDirectoryService
{
    private readonly IConfiguration _config;

    public ActiveDirectoryService(IConfiguration config)
    {
        _config = config;
    }

    private LdapConnection CreateConnection()
    {
        var server = _config["ActiveDirectory:Server"];
        var port = int.Parse(_config["ActiveDirectory:Port"] ?? "636");
        var username = _config["ActiveDirectory:Username"];
        var password = _config["ActiveDirectory:Password"];

        var identifier = new LdapDirectoryIdentifier(server, port);

        var connection = new LdapConnection(identifier)
        {
            Credential = new NetworkCredential(username, password),
            AuthType = AuthType.Basic
        };  

        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = true;

        connection.Bind();

        return connection;
    }

    // user (all cards)
    public Task<List<ADUserDTO>> GetUsersAsync()
    {
        // 1. Open secure LDAPS connection to Active Directory
        using var connection = CreateConnection();

        // 2. Get the base DN from appsettings.json
        // Example: DC=IAM,DC=local
        var baseDn = _config["ActiveDirectory:BaseDn"];

        if (string.IsNullOrWhiteSpace(baseDn))
            throw new Exception("ActiveDirectory:BaseDn is missing in appsettings.json");

        // 3. Build LDAP search request
        // This filter means:
        // - objectCategory=person
        // - objectClass=user
        // So it returns real AD user accounts
        var request = new SearchRequest(
            baseDn,
            "(&(objectCategory=person)(objectClass=user))",
            SearchScope.Subtree,
            "sAMAccountName",
            "displayName",
            "distinguishedName",
            "mail",
            "userAccountControl"

        );

        // 4. Send the LDAP request to Active Directory
        var response = (SearchResponse)connection.SendRequest(request);




        // 5. Convert LDAP result entries into your DTO
        var users = response.Entries
            .Cast<SearchResultEntry>()
            .Select(MapToUserDto)
            .Where(user => !string.IsNullOrWhiteSpace(user.SamAccountName))
            .ToList();

        // 6. Return the final clean list
        return Task.FromResult(users);
    }


    // Search for user(userProfile card) by username
    public Task<ADUserDetailsDTO> GetUserDetailsAsync(string username)
    {
        using var connection = CreateConnection();

        var baseDn = _config["ActiveDirectory:BaseDn"];

        if (string.IsNullOrWhiteSpace(baseDn))
            throw new Exception("ActiveDirectory:BaseDn is missing in appsettings.json");




        var request = new SearchRequest(
            baseDn,
            $"(&(objectCategory=person)(objectClass=user)(sAMAccountName={username}))",
            SearchScope.Subtree,
            "sAMAccountName",
            "displayName",
            "distinguishedName",
            "mail",
            "employeeID",
            "memberOf",
            "userAccountControl",
            "lockoutTime"
        );

        var response = (SearchResponse)connection.SendRequest(request);
        var entry = response.Entries[0];



        var user = MapToUserDetailsDTO(entry);

        // 6. Return the final clean list
        return Task.FromResult(user);

    }
    public Task<bool> DisableUserAsync(string username)
    {
        using var connection = CreateConnection();

        var baseDn = _config["ActiveDirectory:BaseDn"];

        if (string.IsNullOrWhiteSpace(baseDn))
            throw new Exception("ActiveDirectory:BaseDn is missing in appsettings.json");


        var searchRequest = new SearchRequest(
            baseDn,
            $"(&(objectCategory=person)(objectClass=user)(sAMAccountName={username}))",
            SearchScope.Subtree,
            "distinguishedName",
            "userAccountControl"
        );


        var response = (SearchResponse)connection.SendRequest(searchRequest);
        var entry = response.Entries[0];
        //user not found
        if (entry == null)
            return Task.FromResult(false);

        var userAccountControl = GetIntValue(entry, "userAccountControl");


        // Remove ACCOUNTENABLE flag
        var disabledValue = userAccountControl | 2;

        var modifyRequest = new ModifyRequest(
            entry.DistinguishedName,
            DirectoryAttributeOperation.Replace,
            "userAccountControl",
            disabledValue.ToString()
        );
        connection.SendRequest(modifyRequest);

        return Task.FromResult(true);

    }

    public Task<bool> EnableUserAsync(string username)
    {
        using var connection = CreateConnection();

        var baseDn = _config["ActiveDirectory:BaseDn"];

        if (string.IsNullOrWhiteSpace(baseDn))
            throw new Exception("ActiveDirectory:BaseDn is missing in appsettings.json");




        var searchRequest = new SearchRequest(
            baseDn,
            $"(&(objectCategory=person)(objectClass=user)(sAMAccountName={username}))",
            SearchScope.Subtree,
            "distinguishedName",
            "userAccountControl"
        );


        var response = (SearchResponse)connection.SendRequest(searchRequest);
        var entry = response.Entries[0];
        //user not found
        if (entry == null)
            return Task.FromResult(false);

        var userAccountControl = GetIntValue(entry, "userAccountControl");


        // Remove ACCOUNTDISABLE flag
        var enabledValue = userAccountControl & ~2;

        var modifyRequest = new ModifyRequest(
            entry.DistinguishedName,
            DirectoryAttributeOperation.Replace,
            "userAccountControl",
            enabledValue.ToString()
        );
        connection.SendRequest(modifyRequest);

        return Task.FromResult(true);


    }


    // 
    public Task<bool> ResetPasswordAsync(string username, string newPassword)
    {
        using var connection = CreateConnection();

        var baseDn = _config["ActiveDirectory:BaseDn"];

        if (string.IsNullOrWhiteSpace(baseDn))
            throw new Exception("ActiveDirectory:BaseDn is missing in appsettings.json");

        var safeUsername = EscapeLdapFilterValue(username);

        var searchRequest = new SearchRequest(
            baseDn,
            $"(&(objectCategory=person)(objectClass=user)(sAMAccountName={safeUsername}))",
            SearchScope.Subtree,
            "distinguishedName"
        );

        var searchResponse =
            (SearchResponse)connection.SendRequest(searchRequest);

        if (searchResponse.Entries.Count == 0)
            return Task.FromResult(false);

        var userEntry = searchResponse.Entries[0];

        var quotedPassword = $"\"{newPassword}\"";
        var passwordBytes =
            System.Text.Encoding.Unicode.GetBytes(quotedPassword);

        var passwordModification =
            new DirectoryAttributeModification
            {
                Name = "unicodePwd",
                Operation = DirectoryAttributeOperation.Replace
            };

        passwordModification.Add(passwordBytes);

        var modifyRequest =
            new ModifyRequest(userEntry.DistinguishedName);

        modifyRequest.Modifications.Add(passwordModification);

        connection.SendRequest(modifyRequest);

        return Task.FromResult(true);
    }

    //helper methods
    private static ADUserDTO MapToUserDto(SearchResultEntry entry)
    {
        var userAccountControl =
            GetIntValue(entry, "userAccountControl");

        bool enabled = (userAccountControl & 2) == 0;

        return new ADUserDTO(
            GetValue(entry, "sAMAccountName"),
            GetValue(entry, "displayName"),
            entry.DistinguishedName,
            GetValue(entry, "mail"),
            enabled

        );
    }
    private static ADUserDetailsDTO MapToUserDetailsDTO(SearchResultEntry entry)
    {


        var userAccountControl =
            GetIntValue(entry, "userAccountControl");

        var lockoutTime =
            GetLongValue(entry, "lockoutTime");

        bool enabled = (userAccountControl & 2) == 0;

        var lockedOut = lockoutTime > 0;

        return new ADUserDetailsDTO(
            GetValue(entry, "sAMAccountName"),
            GetValue(entry, "displayName"),
            entry.DistinguishedName,
            GetValue(entry, "mail"),
            GetValue(entry, "employeeID"),
            GetValues(entry, "memberOf"),
            enabled,
            lockedOut
        );

    }


    private static string GetValue(SearchResultEntry entry, string attributeName)
    {
        if (!entry.Attributes.Contains(attributeName))
            return "";

        return entry.Attributes[attributeName][0]?.ToString() ?? "";
    }
    private static string[] GetValues(SearchResultEntry entry, string attributeName)
    {
        if (!entry.Attributes.Contains(attributeName))
            return [];

        return entry.Attributes[attributeName]
            .Cast<object>()
            .Select(value =>
            {
                if (value is byte[] bytes)
                    return System.Text.Encoding.UTF8.GetString(bytes);

                return value?.ToString() ?? "";
            })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
    }

    private static int GetIntValue(SearchResultEntry entry, string attributeName)
    {
        var value = GetValue(entry, attributeName);

        return int.TryParse(value, out var result)
            ? result
            : 0;
    }

    private static long GetLongValue(SearchResultEntry entry, string attributeName)
    {
        var value = GetValue(entry, attributeName);

        return long.TryParse(value, out var result)
            ? result
            : 0;
    }

    private static string EscapeLdapFilterValue(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }


}