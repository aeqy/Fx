using Fx.Infrastructure.Date;
using static OpenIddict.Abstractions.OpenIddictConstants.Scopes;

namespace Fx.WebApi.Extensions;

/// <summary>
/// 扩展类 用于配置 OpenIddict
/// </summary>
public static class OpenIddictExtensions
{
    /// <summary>
    /// 为 IServiceCollection 添加并配置 OpenIddict
    /// </summary>
    /// <param name="services">IServiceCollection：依赖注入容器</param>
    /// <param name="configuration">IConfiguration：配置对象，用于获取应用配置</param>
    /// <returns>返回配置后的 IServiceCollection</returns>
    public static IServiceCollection ConfigureOpenIddict(this IServiceCollection services, IConfiguration configuration)
    {
        // 添加 OpenIddict 核心服务及配置
        services.AddOpenIddict()
            .AddCore(options =>
            {
                // 使用 Entity Framework Core 存储 OpenIddict 实体
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>() // 使用应用的 DbContext 管理 OpenIddict 实体
                    .ReplaceDefaultEntities<Guid>(); // 将 OpenIddict 实体的主键类型设为 Guid
            })
            .AddServer(options =>
            {
                // 启用授权模式，允许授权码流、密码流、客户端凭证流和刷新令牌流
                options.AllowAuthorizationCodeFlow()    // 启用授权码流 (Authorization Code Flow)
                    .AllowPasswordFlow()                // 启用密码流 (Password Flow)
                    .AllowClientCredentialsFlow()       // 启用客户端凭证流 (Client Credentials Flow)
                    .AllowRefreshTokenFlow();           // 启用刷新令牌流 (Refresh Token Flow)

                // 配置端点 URI，分别设置令牌和授权端点
                options.SetTokenEndpointUris("/connect/token")  // 设置令牌端点 URI
                    .SetAuthorizationEndpointUris("/connect/authorize"); // 设置授权端点 URI

                // 注册作用域 (scopes)
                // OpenIddict 将允许这些作用域作为授权的一部分
                options.RegisterScopes(OpenId, Profile, Email, OfflineAccess);

                // 添加签名和加密证书
                // 可以使用实际的证书来加密和签名令牌，示例代码已注释
                // options.AddSigningCertificate(certificate);
                // options.AddEncryptionCertificate(certificate);
                
                // 添加临时加密密钥和签名密钥（仅用于开发环境）
                // 在生产环境中，应该使用真实的证书
                options.AddEphemeralEncryptionKey();  // 使用临时加密密钥（仅限开发环境）
                options.AddEphemeralSigningKey();     // 使用临时签名密钥（仅限开发环境）
                
                // 禁用某些功能（可选）
                // 如果不希望启用某些功能，特别是开发过程中，可以使用这些选项
                // options.DisableAccessTokenEncryption();  // 禁用访问令牌加密
                // options.DisableTokenStorage();           // 禁用令牌持久化存储

                // 可以启用开发证书，替代临时密钥
                // options.AddDevelopmentEncryptionCertificate()
                //     .AddDevelopmentSigningCertificate();

                // 配置 ASP.NET Core 端点的处理方式
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough() // 允许通过 ASP.NET Core 的管道返回令牌请求的响应
                    .EnableAuthorizationEndpointPassthrough(); // 允许通过 ASP.NET Core 管道返回授权请求的响应

                // 设置令牌生命周期
                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30)) // 设置访问令牌的有效期为30分钟
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(7)); // 设置刷新令牌的有效期为7天
            })
            .AddValidation(options =>
            {
                // 添加 OpenIddict 验证服务
                // 使用 ASP.NET Core 验证中间件来验证访问令牌
                options.UseAspNetCore();
                
                // 配置验证服务器为本地服务器
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
