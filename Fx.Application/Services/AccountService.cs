using System.Security.Claims;
using Fx.Application.DTOs;
using Fx.Application.Interfaces;
using Fx.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Fx.Application.Services;


/// <summary>
/// 账号服务, 负责账号注册、登录等
/// </summary>
public class AccountService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IOpenIddictTokenManager tokenManager)
    : IAccountService
{
    /// <summary>
    /// 用户注册
    /// </summary>
    public async Task<IdentityResult> RegisterAsync(RegisterDto model)
    {
        var user = new AppUser
        {
            UserName = model.UserName,
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// 用户登陆并生成token,jwt 令牌
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // 创建用户主键
        var principal = await signInManager.CreateUserPrincipalAsync(user);

        // 添加必要的声明
        principal.SetScopes(Scopes.OpenId, Scopes.Profile, Scopes.OfflineAccess);
        principal.SetResources("resource_server");

        // 生成访问令牌
        var accessToken = await GenerateTokenAsync(principal, TokenTypes.Bearer, TimeSpan.FromHours(1));

        // 生成刷新令牌，手动使用 "refresh_token"
        var refreshToken = await GenerateTokenAsync(principal, "refresh_token", TimeSpan.FromDays(30));

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = DateTime.UtcNow.AddHours(1),
        };
    }

    /// <summary>
    /// 生成令牌 (访问令牌/刷新令牌)
    /// </summary>
    private async Task<string> GenerateTokenAsync(ClaimsPrincipal principal, string tokenType, TimeSpan expiresIn)
    {
        // 获取并检查 Subject 声明
        var subject = GetSubject(principal);

        // 创建令牌描述
        var tokenDescriptor = new OpenIddictTokenDescriptor
        {
            Principal = principal,
            Subject = subject,
            Type = tokenType,
            // Type = "refresh_token", // 手动使用 "refresh_token" 字符串
            ExpirationDate = DateTimeOffset.UtcNow.Add(expiresIn)   // 令牌过期时间
        };

        // 生成令牌
        var token = await tokenManager.CreateAsync(tokenDescriptor, CancellationToken.None);

        return token.ToString() ?? string.Empty; // 返回令牌字符串, 如果为空则返回空字符串
    }

    /// <summary>
    /// 获取并检查 Subject 声明
    /// </summary>
    private string GetSubject(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(Claims.Subject);
        if (claim == null || string.IsNullOrEmpty(claim.Value))
        {
            throw new ArgumentNullException(nameof(claim), "The subject claim is missing or invalid.");
        }

        return claim.Value;
    }
}
