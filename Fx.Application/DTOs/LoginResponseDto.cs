namespace Fx.Application.DTOs;

/// <summary>
/// 为了支持返回刷新令牌
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresIn { get; set; }
}