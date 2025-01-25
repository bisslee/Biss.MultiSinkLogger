using Biss.MultiSinkLogger.ExceptionHandlers;
using Biss.MultiSinkLogger.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Biss.MultiSinkLogger.Extensions
{
    /// <summary>
    /// Métodos de extensão para configurar logging e middlewares personalizados.
    /// </summary>
    public static class LoggingServiceExtensions
    {
        /// <summary>
        /// Adiciona o middleware de logging personalizado ao pipeline de requisições.
        /// </summary>
        /// <param name="app">A instância de <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A instância de <see cref="IApplicationBuilder"/> para encadeamento.</returns>

        public static IApplicationBuilder UseCustomLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggingMiddleware>();
            return app;
        }

        /// <summary>
        /// Adiciona o middleware de logging de exceções ao pipeline de requisições.
        /// </summary>
        /// <param name="app">A instância de <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A instância de <see cref="IApplicationBuilder"/> para encadeamento.</returns>

        public static IApplicationBuilder UseExceptionLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionLoggingMiddleware>();
            return app;
        }

        /// <summary>
        /// Adiciona os serviços necessários para o logging personalizado.
        /// </summary>
        /// <param name="services">A coleção de serviços da aplicação.</param>
        /// <param name="useElmahCore">Indica se o ElmahCore deve ser configurado.</param>
        /// <returns>A coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();          
            return services;
        }
    }
}
