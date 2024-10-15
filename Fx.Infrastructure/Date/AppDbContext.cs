using Fx.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Fx.Infrastructure.Date;

/// <summary>
///  数据库上下文
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    
    // 配置 OpenIddict 实体，确保所有实体都使用相同的主键类型 Guid
    public DbSet<OpenIddictEntityFrameworkCoreApplication<Guid>> OpenIddictApplications { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization<Guid>> OpenIddictAuthorizations { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreScope<Guid>> OpenIddictScopes { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreToken<Guid>> OpenIddictTokens { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.UseOpenIddict<Guid>();  // Use OpenIddict
    }
}