using Fx.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Fx.Application.Interfaces;

/// <summary>
/// 定义了注册、登录、以及其他相关的身份验证操作。
/// 在下面的示例中，接口包含用户注册和登录方法，并返回相关的结果，
/// 如注册时的 IdentityResult 或登录时的 JWT 令牌
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// 注册新用户
    /// </summary>
    /// <param name="model">用户注册传输对象</param>
    /// <returns>返回注册结果</returns>
    Task<IdentityResult> RegisterAsync(RegisterDto model);
    
    /// <summary>
    /// 登录用户并生成JWT 令牌
    /// </summary>
    /// <param name="model">用户登录传输对象</param>
    /// <returns>JWT 令牌</returns>
    Task<LoginResponseDto> LoginAsync(LoginDto model);
    
}