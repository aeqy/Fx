using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fx.Infrastructure.Date;

// 实现 IDesignTimeDbContextFactory 接口
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 获取当前目录的上级目录（解决 Web API 和 Infrastructure 项目不同目录的问题）
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Fx.WebApi");

        try
        {
            // 创建配置对象
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // 支持读取基于环境的配置文件，比如 appsettings.Development.json
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            // 读取 PostgreSQL 连接字符串
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("未配置连接字符串。请检查 appsettings.json 中的 ConnectionStrings:DefaultConnection 设置。");
            }

            // 配置 DbContextOptions 使用 PostgreSQL
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // 可选：添加日志支持，方便调试和生产环境监控
            optionsBuilder.UseNpgsql(connectionString)
                .EnableSensitiveDataLogging() // 开启敏感数据日志（仅用于开发环境，生产环境需关闭）
                .UseLoggerFactory(LoggerFactory.Create(builder => 
                {
                    builder.AddConsole(); // 将日志输出到控制台
                }));

            // 返回配置后的 AppDbContext 实例
            return new AppDbContext(optionsBuilder.Options);
        }
        catch (Exception ex)
        {
            // 捕获并抛出配置或 DbContext 创建过程中出现的异常
            throw new InvalidOperationException($"无法创建 DbContext：{ex.Message}", ex);
        }
    }
}