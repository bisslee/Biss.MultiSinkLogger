using Biss.MultiSinkLogger.Configuration;
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.ExceptionHandlers;
using Biss.MultiSinkLogger.HealthChecks;
using Biss.MultiSinkLogger.Middleware;
using Biss.MultiSinkLogger.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

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
        /// <returns>A coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();
            RegisterSinkConfigurators(services);
            return services;
        }

        /// <summary>
        /// Adiciona os serviços necessários para o logging personalizado com validação de configuração.
        /// </summary>
        /// <param name="services">A coleção de serviços da aplicação.</param>
        /// <param name="configuration">A configuração da aplicação.</param>
        /// <returns>A coleção de serviços para encadeamento.</returns>
        public static IServiceCollection AddCustomLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurar e validar LoggerManagerSettings
            services.Configure<LoggerManagerSettings>(
                configuration.GetSection(nameof(LoggerManagerSettings)));

            // Registrar validador
            services.AddSingleton<IValidateOptions<LoggerManagerSettings>, LoggerManagerSettingsValidator>();

            // Validar na inicialização
            services.AddOptions<LoggerManagerSettings>()
                .Bind(configuration.GetSection(nameof(LoggerManagerSettings)))
                .ValidateOnStart();

            services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();

            // Registrar todos os configuradores de sink
            RegisterSinkConfigurators(services);

            // Registrar factory que usa DI
            services.AddSingleton<SinkConfiguratorFactory>(sp =>
            {
                var configurators = sp.GetServices<ISinkConfigurator>();
                return new SinkConfiguratorFactory(configurators);
            });

            // Registrar health check
            services.AddHealthChecks()
                .AddCheck<SinkHealthCheck>(
                    "multi_sink_logger",
                    tags: new[] { "logging", "sinks" });

            // Configurar settings do middleware (com valores padrão)
            services.Configure<LoggingMiddlewareSettings>(options =>
            {
                configuration.GetSection("LoggingMiddleware").Bind(options);
            });

            return services;
        }

        /// <summary>
        /// Registra todos os configuradores de sink no container de DI.
        /// </summary>
        /// <param name="services">A coleção de serviços.</param>
        private static void RegisterSinkConfigurators(IServiceCollection services)
        {
            // Registrar todos os configuradores como singletons
            services.AddSingleton<ISinkConfigurator, ConsoleSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, FileSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, SqlServerSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, SqliteSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, PostgreSqlSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, MySqlSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, MongoDBSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, CosmosDBSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, RabbitMQSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, SentrySinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, SlackSinkConfigurator>();
            services.AddSingleton<ISinkConfigurator, NewRelicSinkConfigurator>();
        }
    }
}
