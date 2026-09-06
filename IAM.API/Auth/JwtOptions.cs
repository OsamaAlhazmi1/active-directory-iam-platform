using System;

namespace IAM_API.Auth;

public class JwtOptions
{
    public string Key { get; set; } = "";
    public int ExpiryMinutes { get; set; }
}
