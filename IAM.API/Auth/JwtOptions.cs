using System;

namespace AD_web_project.Auth;

public class JwtOptions
{
    public string Key { get; set; } = "";
    public int ExpiryMinutes { get; set; }
}
