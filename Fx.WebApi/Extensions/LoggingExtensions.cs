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
}