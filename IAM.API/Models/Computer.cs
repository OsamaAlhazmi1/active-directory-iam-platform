

namespace IAM_API.Models;

public class Computer
{
    public int Id { get; set; }

    public string ComputerName { get; set; } = "";

    public List<ComputerPermission> ComputerPermissions { get; set; } = new();
    public List<GroupComputer> GroupComputers { get; set; } = new();
}
