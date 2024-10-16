using System.Security.Cryptography.X509Certificates;
using Fx.Application.Interfaces;
using Fx.Application.Services;
using Fx.Domain.Entities;
using Fx.Infrastructure.Date;
using Fx.WebApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 配置日志记录
builder.Logging.ConfigureLogging();

// 加载配置文件
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); 

// 配置数据库和 Identity
builder.Services.ConfigureDatabaseAndIdentity(builder.Configuration);

// 使用扩展方法来配置 OpenIddict
builder.Services.ConfigureOpenIddict(builder.Configuration);

// 配置 JWT 验证
builder.Services.ConfigureAuthentication(builder.Configuration);

// 注入业务服务, 依赖注入 IAccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// 注入种子数据服务
builder.Services.AddScoped<SeedDataService>();



builder.Services.AddControllers();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 Swagger UI
app.UseSwaggerWithUi(app.Environment);

// 执行种子数据
await  app.SeedDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();    // 先显示异常信息
}

// 使用 CORS 策略
// app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection(); // 先重定向

// app.UseStaticFiles(); //先加载静态文件
app.UseRouting(); // 先注册路由
app.UseAuthentication(); //先验证身份
app.UseAuthorization(); //后验证权限

app.MapControllers(); //顶级路由注册

app.Run();