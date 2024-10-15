using Microsoft.AspNetCore.Identity;

namespace Fx.Domain.Entities;

/// <summary>
///  用户实体, 继承自 IdentityUser, 使用 Guid 做主键
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    // /// <summary>
    // ///  手机号, 唯一
    // /// </summary>
    // public string PhoneNumber { get; private set; }
    //
    // /// <summary>
    // ///  设置手机号
    // /// </summary>
    // /// <param name="phoneNumber"></param>
    // /// <exception cref="ArgumentException"></exception>
    // public void SetPhoneNumber(string phoneNumber)
    // {
    //     if (string.IsNullOrWhiteSpace(phoneNumber))
    //     {
    //         throw  new ArgumentException("手机号不能为空");
    //     }
    //     PhoneNumber = phoneNumber;
    // }
}