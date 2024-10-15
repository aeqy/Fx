using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Fx.WebApi.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// 扩展方法，用于为 IServiceCollection 配置 JWT 认证。
    /// 该方法为应用程序添加基于 JWT 的身份验证。
    /// </summary>
    /// <param name="services">IServiceCollection：依赖注入容器</param>
    /// <param name="configuration">IConfiguration：配置对象，用于获取应用配置</param>
    /// <returns>返回配置后的 IServiceCollection</returns>
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置认证服务，指定使用 JWT Bearer 作为默认的认证方案
        services.AddAuthentication(options =>
            {
                // 设置默认的认证方案为 JWT Bearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // 设置默认的挑战方案为 JWT Bearer，意味着认证失败时会使用此方案
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // 配置 Authority，即颁发 JWT 的授权服务器地址
                // 在该示例中，它指向本地的 Identity 或授权服务器
                options.Authority = "https://localhost:7268"; 

                // 配置 Audience，指定 JWT 的受众（即此 API），通常用于验证令牌的受众字段
                options.Audience = "api";

                // 开发环境下禁用 HTTPS 元数据验证
                // 在生产环境中应该启用此功能以确保安全性
                options.RequireHttpsMetadata = false;
            });
        
        // 返回服务集合，支持链式调用
        return services;
    }
}