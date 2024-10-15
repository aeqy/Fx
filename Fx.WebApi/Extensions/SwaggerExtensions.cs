namespace Fx.WebApi.Extensions;

public static class SwaggerExtensions
{
    // 扩展方法，用于在应用程序中启用并配置 Swagger 和 Swagger UI
    public static IApplicationBuilder UseSwaggerWithUi(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 仅在开发环境中启用 Swagger 和 Swagger UI
        if (env.IsDevelopment())
        {
            // 启用 Swagger 中间件，生成 API 文档
            app.UseSwagger();

            // 配置 Swagger UI 页面
            app.UseSwaggerUI(c =>
            {
                // 设置 Swagger JSON 端点，提供 API 文档
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

                // 将 Swagger UI 的根路径配置为 "swagger"，即 Swagger UI 将在 "http://localhost:端口/swagger" 处访问
                c.RoutePrefix = "swagger"; 

                // 开启深色模式支持，允许用户在 UI 中切换为深色模式
                c.EnableDeepLinking(); 

                // 配置 OAuth2 的客户端 ID，用于 OAuth2 认证
                c.OAuthClientId("your-client-secret"); 

                // 设置 Swagger UI 上显示的应用名称
                c.OAuthAppName("My API - Swagger");

                // 启用 PKCE (Proof Key for Code Exchange)，提高 OAuth2 的安全性
                c.OAuthUsePkce(); 

                // 配置文档折叠模式，按列表展示 API 文档；可以调整为 "None"（折叠所有 API）或 "Full"（展开所有 API）
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                // 设置模型展开的深度，值越大表示展开更多层次的属性
                c.DefaultModelExpandDepth(2); 

                // 设置模型默认的渲染方式，使用 Model 渲染（另一种选择是 Schema）
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model); 

                // 默认启用 "Try it out" 按钮，允许用户在 Swagger UI 中直接调用 API
                c.EnableTryItOutByDefault(); 

                // 可以注入自定义的 CSS 样式，用于修改 Swagger UI 的外观
                // c.InjectStylesheet("/swagger-ui/custom.css"); 

                // 限制 Swagger UI 中支持的 HTTP 提交方法（如 GET、POST、PUT），可以只显示特定方法
                // c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put);
            });
        }

        return app; // 返回配置好的中间件
    }
}
