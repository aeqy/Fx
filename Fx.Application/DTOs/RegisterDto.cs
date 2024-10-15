namespace Fx.Application.DTOs;

/// <summary>
/// 注册信息, 包含用户名, 邮箱, 密码
/// </summary>
public class RegisterDto
{
    public string UserName { get; set; }    //用户名
    // public string Email { get; set; }       //邮箱
    public string Password { get; set; }    //密码

    // 可选择, 但必须与密码一致
    public string ConfirmPassword { get; set; } //确认密码
}