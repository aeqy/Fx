using Fx.Domain.Entities;
using Fx.Infrastructure.Date;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fx.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 配置数据库和身份验证服务。
    /// 该方法用于在应用程序启动时注册数据库上下文、身份验证和 OpenIddict 服务。
    /// </summary>
    /// <param name="services">IServiceCollection 用于注册服务的容器</param>
    /// <param name="configuration">IConfiguration 提供应用程序的配置值</param>
    /// <returns>返回 IServiceCollection 以便支持链式调用</returns>
    public static IServiceCollection ConfigureDatabaseAndIdentity(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置数据库上下文，使用 PostgreSQL 数据库
        services.AddDbContext<AppDbContext>(options =>
        {
            // 从配置文件中获取数据库连接字符串，使用 Npgsql 来连接 PostgreSQL 数据库
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            // 为 DbContext 配置 OpenIddict，使用 Guid 作为主键类型，用于处理身份验证的相关操作
            options.UseOpenIddict<Guid>();
        });

        // 配置 ASP.NET Core Identity 用于用户管理和身份验证
        // 添加 Identity 服务，指定 AppUser 作为用户实体，IdentityRole<Guid> 作为角色实体，使用 Guid 作为主键
        services.AddIdentity<AppUser, IdentityRole<Guid>>()
            // 将 Identity 存储配置为使用 Entity Framework Core 存储用户和角色信息
            .AddEntityFrameworkStores<AppDbContext>()
            // 添加默认的令牌提供程序，用于生成密码重置令牌、电子邮件确认令牌等
            .AddDefaultTokenProviders();

        // 返回服务集合，以支持链式配置调用
        return services;
    }
}