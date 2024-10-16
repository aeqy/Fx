using Serilog;

namespace Fx.WebApi.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// 配置日志记录。
    /// 此方法用于配置应用程序中的日志记录功能，并为其添加多个日志提供程序。
    /// </summary>
    /// <param name="logging">用于构建和配置日志记录的 ILoggingBuilder 实例。</param>
    /// <returns>返回配置好的 ILoggingBuilder，支持链式调用。</returns>
    public static ILoggingBuilder ConfigureLogging(this ILoggingBuilder logging)
    {
        // 清除所有现有的日志提供程序，以便重新配置日志
        logging.ClearProviders() // 例如，默认的 Console、Debug 等日志提供程序都将被清除

            // 添加控制台日志提供程序，使日志信息输出到控制台
            .AddConsole() 

            // 添加调试日志提供程序，使日志信息输出到调试窗口（例如在 Visual Studio 中的调试输出窗口）
            .AddDebug()

            // 过滤 OpenIddict 相关的日志，将其日志级别设置为 Debug
            // 这样可以更详细地记录 OpenIddict 相关的调试信息
            .AddFilter("OpenIddict", LogLevel.Debug);   

        // 返回配置好的 ILoggingBuilder，支持进一步的配置或调用链
        return logging;
    }

    /// <summary>
    /// 配置 Serilog，用于记录应用程序日志
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)  // 从配置文件中读取 Serilog 配置
                .Enrich.FromLogContext()                // 从日志上下文中提取信息
                .Enrich.WithMachineName()               // 添加机器名称
                .Enrich.WithProcessId()                 // 添加进程 ID
                .Enrich.WithThreadId()                  // 添加线程 ID
                .WriteTo.Console();                     // 将日志输出到控制台
                // .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:Uri"] ?? string.Empty))
                // {
                //     AutoRegisterTemplate = true,
                //     IndexFormat = $"{configuration["Elasticsearch:IndexPrefix"]}-logs-{DateTime.UtcNow:yyyy-MM}"
                // });
        });
    }
}