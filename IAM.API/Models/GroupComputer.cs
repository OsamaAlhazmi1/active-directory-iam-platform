using System;

namespace AD_web_project.Models;

public class GroupComputer
{
    public int Id { get; set; }
    public int ComputerId { get; set; }
    public Computer Computer { get; set; } = null!;
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

}