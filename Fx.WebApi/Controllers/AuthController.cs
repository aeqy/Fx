using System.Security.Claims;
using Fx.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Fx.WebApi.Controllers
{
    // 认证控制器，处理与 OpenID Connect 和 OAuth2 相关的令牌请求
    public class AuthController(UserManager<AppUser> userManager) : Controller
    {
        // 通过构造函数注入 UserManager<AppUser> 用于管理用户身份
        // UserManager 用于用户的查找和密码验证等操作

        [HttpPost(template: "/connect/token")]
        // 该方法用于处理令牌请求，如访问令牌和刷新令牌
        public async Task<IActionResult> Exchange()
        {
            // 从当前 HTTP 请求中获取 OpenIddict 的请求对象，确保是有效的 OpenID Connect 请求
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                // 如果请求无效，返回友好的错误信息
                return BadRequest(new OpenIddictResponse
                {
                    Error = Errors.InvalidRequest,
                    ErrorDescription = "The OpenID Connect request cannot be retrieved."
                });
            }

            // 处理密码模式授权请求 (Password Grant)
            if (request.IsPasswordGrantType())
            {
                if (request.Username != null)
                {
                    // 查找用户对象，检查用户名是否存在
                    var user = await userManager.FindByNameAsync(request.Username);
                    
                    // 如果用户不存在，或密码验证失败，返回 InvalidGrant 错误
                    if (request.Password != null && (user == null || !await userManager.CheckPasswordAsync(user, request.Password)))
                    {
                        return BadRequest(new OpenIddictResponse
                        {
                            Error = Errors.InvalidGrant,
                            ErrorDescription = "The username or password is incorrect."
                        });
                    }

                    // 创建一个包含用户身份的 ClaimsIdentity，表示用户的认证状态
                    var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                    // 添加用户的唯一标识符 (Subject) 声明
                    identity.AddClaim(Claims.Subject, user?.Id.ToString() ?? string.Empty);

                    // 如果用户名不为空，添加用户名声明
                    if (user is { UserName: not null }) 
                        identity.AddClaim(Claims.Name, user.UserName);

                    // 设置声明目标，指定哪些声明将包含在访问令牌和身份令牌中
                    identity.SetDestinations(claim => claim.Type switch
                    {
                        // 将 Subject 和 Name 声明包含在访问令牌和身份令牌中
                        Claims.Name or Claims.Subject => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                        // 其他声明仅包含在访问令牌中
                        _ => new[] { Destinations.AccessToken }
                    });

                    // 创建 ClaimsPrincipal，用于代表该用户的身份声明集合
                    var principal = new ClaimsPrincipal(identity);

                    // 设置令牌范围 (Scopes)，指定客户端请求的权限
                    principal.SetScopes(Scopes.OfflineAccess, Scopes.OpenId, Scopes.Profile);

                    // 使用 OpenIddict 的 SignIn 方法生成并返回访问令牌和刷新令牌
                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
            }

            // 处理刷新令牌模式授权请求 (Refresh Token Grant)
            if (request.IsRefreshTokenGrantType())
            {
                // 验证现有的刷新令牌
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                if (result.Principal == null)
                {
                    // 刷新令牌无效，返回 InvalidGrant 错误
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "The refresh token is no longer valid."
                    });
                }

                // 获取现有的用户身份声明 (ClaimsPrincipal)
                var principal = result.Principal;

                // 设置新的令牌范围
                principal.SetScopes(Scopes.OfflineAccess, Scopes.OpenId, Scopes.Profile);

                // 生成并返回新的访问令牌
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // 如果请求的授权类型不支持，返回 UnsupportedGrantType 错误
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }
    }
}
