using System;
using AD_web_project.Models;
using Microsoft.EntityFrameworkCore;

namespace AD_web_project.Data;

public class LocalContext(DbContextOptions<LocalContext> options)
    : DbContext(options)
{
    public DbSet<ActionLog> ActionLogs => Set<ActionLog>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Computer> Computers => Set<Computer>();

    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<ComputerPermission> ComputerPermissions => Set<ComputerPermission>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<ActionRule> ActionRules => Set<ActionRule>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupPermission> GroupPermissions => Set<GroupPermission>();
    public DbSet<GroupUser> GroupUsers => Set<GroupUser>();
    public DbSet<GroupComputer> GroupComputers => Set<GroupComputer>();



}
