# Guia de Implementação de Melhorias - Biss.MultiSinkLogger

Este documento contém exemplos de código para implementar as melhorias críticas identificadas na análise arquitetural.

---

## 1. Thread Safety na Inicialização

### Problema
A inicialização do logger não é thread-safe, podendo causar race conditions.

### Solução

**Arquivo:** `LoggingManager.cs`

```csharp
using Biss.MultiSinkLogger.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Biss.MultiSinkLogger
{
    public static class LoggingManager
    {
        private static readonly object _lock = new object();
        private static bool _initialized = false;
        private static ILogger? _logger;

        public static void InitializeLogger(IConfiguration configuration)
        {
            if (_initialized && _logger != null) return;
            
            lock (_lock)
            {
                if (_initialized && _logger != null) return;
                
                _logger = new LoggerConfiguration()
                    .ConfigureLogging(configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();
                
                Log.Logger = _logger;
                _initialized = true;
            }
        }

        public static void CloseAndFlush()
        {
            lock (_lock)
            {
                Log.CloseAndFlush();
                _initialized = false;
                _logger = null;
            }
        }
    }
}
```

---

## 2. Validação de Configuração com IValidateOptions

### Problema
Não há validação adequada das configurações antes de usar.

### Solução

**Novo arquivo:** `Configuration/LoggerManagerSettingsValidator.cs`

```csharp
using Biss.MultiSinkLogger.Entities;
using Microsoft.Extensions.Options;

namespace Biss.MultiSinkLogger.Configuration
{
    public class LoggerManagerSettingsValidator : IValidateOptions<LoggerManagerSettings>
    {
        public ValidateOptionsResult Validate(string? name, LoggerManagerSettings options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("LoggerManagerSettings não pode ser null.");
            }

            var errors = new List<string>();

            // Validar MinimumLevel
            if (string.IsNullOrWhiteSpace(options.MinimumLevel))
            {
                errors.Add("MinimumLevel é obrigatório.");
            }
            else if (!Enum.TryParse<Serilog.Events.LogEventLevel>(options.MinimumLevel, true, out _))
            {
                errors.Add($"MinimumLevel '{options.MinimumLevel}' não é um valor válido.");
            }

            // Validar Sinks
            if (options.Sinks == null)
            {
                errors.Add("Sinks não pode ser null.");
            }
            else
            {
                var activeSinks = options.Sinks.Where(s => s.Active).ToList();
                
                if (!activeSinks.Any())
                {
                    errors.Add("Pelo menos um sink deve estar ativo.");
                }

                foreach (var sink in activeSinks)
                {
                    if (string.IsNullOrWhiteSpace(sink.Type))
                    {
                        errors.Add("Tipo de sink não pode ser vazio para sinks ativos.");
                    }

                    if (sink.Settings == null)
                    {
                        errors.Add($"Configurações do sink '{sink.Type}' não podem ser null.");
                    }
                }
            }

            if (errors.Any())
            {
                return ValidateOptionsResult.Fail(errors);
            }

            return ValidateOptionsResult.Success;
        }
    }
}
```

**Atualizar:** `Extensions/LoggingServiceExtensions.cs`

```csharp
using Biss.MultiSinkLogger.Configuration;
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.ExceptionHandlers;
using Biss.MultiSinkLogger.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Biss.MultiSinkLogger.Extensions
{
    public static class LoggingServiceExtensions
    {
        public static IServiceCollection AddCustomLogging(
            this IServiceCollection services,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Configurar e validar LoggerManagerSettings
            services.Configure<LoggerManagerSettings>(
                configuration.GetSection(nameof(LoggerManagerSettings)));
            
            services.AddSingleton<IValidateOptions<LoggerManagerSettings>, LoggerManagerSettingsValidator>();
            
            // Validar na inicialização
            services.AddOptions<LoggerManagerSettings>()
                .Bind(configuration.GetSection(nameof(LoggerManagerSettings)))
                .ValidateOnStart();

            services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();
            
            return services;
        }

        // ... outros métodos ...
    }
}
```

---

## 3. Interface para Logger (Melhorar Testabilidade)

### Problema
Classe estática `Logger` não pode ser mockada em testes.

### Solução

**Novo arquivo:** `ILoggerService.cs`

```csharp
namespace Biss.MultiSinkLogger
{
    /// <summary>
    /// Interface para serviços de logging.
    /// </summary>
    public interface ILoggerService
    {
        void Info(string message);
        void Info(string message, params object[] propertyValues);
        void Debug(string message);
        void Error(string message, Exception? ex = null);
        void Warning(string message);
        void Warning(string message, params object[] propertyValues);
        void Fatal(string message, Exception? ex = null);
    }
}
```

**Atualizar:** `Logger.cs` para implementar a interface

```csharp
using Serilog;

namespace Biss.MultiSinkLogger
{
    /// <summary>
    /// Implementação padrão do serviço de logging usando Serilog.
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }

        public void Info(string message, params object[] propertyValues)
        {
            _logger.Information(message, propertyValues);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                _logger.Error(message);
            }
            else
            {
                _logger.Error(ex, message);
            }
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Warning(string message, params object[] propertyValues)
        {
            _logger.Warning(message, propertyValues);
        }

        public void Fatal(string message, Exception? ex = null)
        {
            if (ex == null)
            {
                _logger.Fatal(message);
            }
            else
            {
                _logger.Fatal(ex, message);
            }
        }
    }

    /// <summary>
    /// Classe estática para compatibilidade com versões anteriores.
    /// </summary>
    [Obsolete("Use ILoggerService via dependency injection instead.")]
    public static class Logger
    {
        public static void Info(string message) => Log.Information(message);
        public static void Info(string message, string attrib) => Log.Information(message, attrib);
        public static void Debug(string message) => Log.Debug(message);
        public static void Error(string message, Exception? ex = null)
        {
            if (ex == null)
                Log.Error(message);
            else
                Log.Error(ex, message);
        }
        public static void Warning(string message) => Log.Warning(message);
        public static void Warning(string message, string? attrib) => Log.Warning(message, attrib);
        public static void Fatal(string message, Exception? ex = null)
        {
            if (ex == null)
                Log.Fatal(message);
            else
                Log.Fatal(ex, message);
        }
    }
}
```

**Atualizar:** `Extensions/LoggingServiceExtensions.cs`

```csharp
public static IServiceCollection AddCustomLogging(
    this IServiceCollection services,
    Microsoft.Extensions.Configuration.IConfiguration configuration)
{
    // ... código anterior ...

    // Registrar ILoggerService
    services.AddSingleton<ILoggerService>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger>();
        return new LoggerService(logger);
    });

    return services;
}
```

---

## 4. Filtro de Dados Sensíveis

### Problema
Headers e body podem conter informações sensíveis (tokens, senhas).

### Solução

**Novo arquivo:** `Security/SensitiveDataFilter.cs`

```csharp
using System.Text.RegularExpressions;

namespace Biss.MultiSinkLogger.Security
{
    /// <summary>
    /// Filtra dados sensíveis de logs.
    /// </summary>
    public class SensitiveDataFilter
    {
        private static readonly string[] SensitiveHeaders = new[]
        {
            "Authorization",
            "X-API-Key",
            "X-Auth-Token",
            "Cookie",
            "Set-Cookie"
        };

        private static readonly string[] SensitivePatterns = new[]
        {
            @"(?i)(password|passwd|pwd)\s*[:=]\s*[""']?([^""'\s]+)[""']?",
            @"(?i)(token|apikey|api_key|access_token)\s*[:=]\s*[""']?([^""'\s]+)[""']?",
            @"(?i)(secret|secretkey|secret_key)\s*[:=]\s*[""']?([^""'\s]+)[""']?",
            @"(?i)(creditcard|cc|cardnumber)\s*[:=]\s*[""']?([^""'\s]+)[""']?"
        };

        /// <summary>
        /// Filtra headers sensíveis de um dicionário.
        /// </summary>
        public static Dictionary<string, string> FilterHeaders(Dictionary<string, string> headers)
        {
            var filtered = new Dictionary<string, string>(headers);
            
            foreach (var sensitiveHeader in SensitiveHeaders)
            {
                if (filtered.ContainsKey(sensitiveHeader))
                {
                    filtered[sensitiveHeader] = "[REDACTED]";
                }
            }

            return filtered;
        }

        /// <summary>
        /// Filtra dados sensíveis de uma string (body, query string, etc.).
        /// </summary>
        public static string FilterSensitiveData(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            var filtered = content;

            foreach (var pattern in SensitivePatterns)
            {
                filtered = Regex.Replace(
                    filtered,
                    pattern,
                    match => $"{match.Groups[1].Value}: [REDACTED]",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return filtered;
        }

        /// <summary>
        /// Sanitiza uma connection string removendo credenciais.
        /// </summary>
        public static string SanitizeConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return connectionString;

            // Remove password, pwd, user id, etc.
            var sanitized = Regex.Replace(
                connectionString,
                @"(?i)(password|pwd|user\s*id|uid)\s*=\s*[^;]+",
                match => $"{match.Groups[1].Value}=[REDACTED]",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return sanitized;
        }
    }
}
```

**Atualizar:** `Middleware/LoggingMiddleware.cs`

```csharp
// ... imports ...
using Biss.MultiSinkLogger.Security;

namespace Biss.MultiSinkLogger.Middleware
{
    public class LoggingMiddleware
    {
        // ... código existente ...

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                context.Request.EnableBuffering();
                var requestBody = await ReadAndTruncateAsync(context.Request.Body);
                
                // Filtrar dados sensíveis
                var filteredHeaders = SensitiveDataFilter.FilterHeaders(
                    context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
                var filteredBody = SensitiveDataFilter.FilterSensitiveData(requestBody);

                var requestLog = new
                {
                    TraceId = context.TraceIdentifier,
                    context.Request.Scheme,
                    context.Request.Host,
                    context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    Headers = filteredHeaders,
                    Body = filteredBody
                };

                Log.Information(LogMessages.HandlingRequest, requestLog);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to log request details.");
            }

            // ... resto do código ...
        }
    }
}
```

---

## 5. Strategy Pattern para Sink Configuration

### Problema
Switch gigante viola Open/Closed Principle.

### Solução

**Atualizar:** `Sinks/ISinkConfigurator.cs`

```csharp
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public interface ISinkConfigurator
    {
        /// <summary>
        /// Obtém o tipo de sink que este configurador suporta.
        /// </summary>
        SinkType SupportedSinkType { get; }

        /// <summary>
        /// Configura o sink no logger.
        /// </summary>
        void Configure(
            LoggerConfiguration loggerConfiguration, 
            ISinkSettings settings, 
            LoggerManagerSettings loggerConfig);

        /// <summary>
        /// Valida as configurações do sink.
        /// </summary>
        void ValidateSettings(ISinkSettings settings);
    }
}
```

**Atualizar:** `Sinks/ConsoleSinkConfigurator.cs`

```csharp
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Biss.MultiSinkLogger.Sinks
{
    public class ConsoleSinkConfigurator : ISinkConfigurator
    {
        public SinkType SupportedSinkType => SinkType.Console;

        public void Configure(
            LoggerConfiguration loggerConfiguration, 
            ISinkSettings settings, 
            LoggerManagerSettings loggerConfig)
        {
            ValidateSettings(settings);
            
            var consoleSettings = (ConsoleSinkSettings)settings;
            var theme = AnsiConsoleTheme.Code;

            loggerConfiguration.WriteTo.Console(
                outputTemplate: loggerConfig.OutputTemplate,
                theme: theme
            );
        }

        public void ValidateSettings(ISinkSettings settings)
        {
            if (settings is not ConsoleSinkSettings)
            {
                throw new ArgumentException(
                    $"Expected {nameof(ConsoleSinkSettings)}, got {settings?.GetType().Name}",
                    nameof(settings));
            }
        }
    }
}
```

**Atualizar:** `Sinks/SinkConfiguratorFactory.cs` para usar registro dinâmico

```csharp
using Biss.MultiSinkLogger.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Biss.MultiSinkLogger.Sinks
{
    public class SinkConfiguratorFactory
    {
        private readonly Dictionary<SinkType, ISinkConfigurator> _configurators;

        public SinkConfiguratorFactory(IEnumerable<ISinkConfigurator> configurators)
        {
            _configurators = configurators.ToDictionary(c => c.SupportedSinkType);
        }

        public ISinkConfigurator GetConfigurator(SinkType sinkType)
        {
            if (!_configurators.TryGetValue(sinkType, out var configurator))
            {
                throw new NotSupportedException($"Sink '{sinkType}' is not supported.");
            }

            return configurator;
        }

        // Método estático para compatibilidade (deprecated)
        [Obsolete("Use dependency injection instead.")]
        public static ISinkConfigurator GetConfigurator(SinkType sinkType)
        {
            return sinkType switch
            {
                SinkType.Console => new ConsoleSinkConfigurator(),
                SinkType.File => new FileSinkConfigurator(),
                // ... outros ...
                _ => throw new NotSupportedException($"Sink '{sinkType}' is not supported.")
            };
        }
    }
}
```

**Atualizar:** `Extensions/LoggingServiceExtensions.cs` para registrar configuradores

```csharp
public static IServiceCollection AddCustomLogging(
    this IServiceCollection services,
    Microsoft.Extensions.Configuration.IConfiguration configuration)
{
    // Registrar todos os configuradores de sink
    services.AddSingleton<ISinkConfigurator, ConsoleSinkConfigurator>();
    services.AddSingleton<ISinkConfigurator, FileSinkConfigurator>();
    services.AddSingleton<ISinkConfigurator, SqlServerSinkConfigurator>();
    // ... outros configuradores ...

    // Registrar factory
    services.AddSingleton<SinkConfiguratorFactory>();

    // ... resto do código ...
}
```

**Atualizar:** `Configuration/SerilogConfiguration.cs` para usar factory via DI

```csharp
// Em vez de usar SinkConfiguratorFactory.GetConfigurator estático,
// injetar SinkConfiguratorFactory via construtor ou service locator
```

---

## 6. Options Pattern

### Problema
Configuração lida diretamente de IConfiguration.

### Solução

**Atualizar:** `Configuration/SerilogConfiguration.cs`

```csharp
using Biss.MultiSinkLogger.Entities;
using Biss.MultiSinkLogger.Entities.TypeSinkSettings;
using Biss.MultiSinkLogger.Exceptions;
using Biss.MultiSinkLogger.Sinks;
using Microsoft.Extensions.Options;
using Serilog;

namespace Biss.MultiSinkLogger.Configuration
{
    public static class SerilogConfiguration
    {
        public static LoggerConfiguration ConfigureLogging(
            this LoggerConfiguration loggerConfiguration,
            IConfiguration configuration,
            SinkConfiguratorFactory? sinkFactory = null)
        {
            var loggerConfig = configuration.GetSection(nameof(LoggerManagerSettings))
                .Get<LoggerManagerSettings>();

            if (loggerConfig == null)
                throw new LoggerConfigurationException("Configuração de Logger não encontrada.");

            // Usar factory injetada ou fallback para estático
            var factory = sinkFactory ?? new SinkConfiguratorFactory(GetDefaultConfigurators());

            SinkSettingsConfig(configuration, loggerConfig, factory);

            loggerConfiguration.MinimumLevel.Is(
                Enum.Parse<Serilog.Events.LogEventLevel>(loggerConfig.MinimumLevel, true));

            if (!loggerConfig.Sinks.Any(s => s.Active))
            {
                loggerConfiguration
                    .WriteTo
                    .Console(outputTemplate: TemplateConstants.OUTPUT_TEMPLATE);
                return loggerConfiguration;
            }

            ConfigureSinkSettings(loggerConfiguration, loggerConfig, factory);

            return loggerConfiguration;
        }

        private static IEnumerable<ISinkConfigurator> GetDefaultConfigurators()
        {
            yield return new ConsoleSinkConfigurator();
            yield return new FileSinkConfigurator();
            // ... outros ...
        }

        // ... resto do código atualizado para usar factory ...
    }
}
```

---

## 7. Health Checks

### Problema
Não há verificação se os sinks estão funcionando.

### Solução

**Novo arquivo:** `HealthChecks/SinkHealthCheck.cs`

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace Biss.MultiSinkLogger.HealthChecks
{
    public class SinkHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar se o logger está inicializado
                if (Log.Logger == null)
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy("Logger não foi inicializado."));
                }

                // Testar escrita de log
                Log.Information("Health check test log");
                Log.CloseAndFlush();

                return Task.FromResult(HealthCheckResult.Healthy("Logger está funcionando corretamente."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Erro ao verificar saúde do logger.", ex));
            }
        }
    }
}
```

**Atualizar:** `Extensions/LoggingServiceExtensions.cs`

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Biss.MultiSinkLogger.HealthChecks;

public static IServiceCollection AddCustomLogging(
    this IServiceCollection services,
    Microsoft.Extensions.Configuration.IConfiguration configuration)
{
    // ... código anterior ...

    // Adicionar health check
    services.AddHealthChecks()
        .AddCheck<SinkHealthCheck>("multi_sink_logger");

    return services;
}
```

---

## 8. Melhorias no DefaultExceptionHandler

### Problema
`DefaultExceptionHandler` não faz nada.

### Solução

**Atualizar:** `ExceptionHandlers/DefaultExceptionHandler.cs`

```csharp
using Microsoft.AspNetCore.Http;
using Serilog;
using Biss.MultiSinkLogger.Constants;

namespace Biss.MultiSinkLogger.ExceptionHandlers
{
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log estruturado com contexto
            Log.Error(
                exception,
                LogMessages.UnhandledException + " TraceId: {TraceId}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Path);

            // Adicionar informações ao contexto de log
            LogContext.PushProperty("TraceId", context.TraceIdentifier);
            LogContext.PushProperty("Path", context.Request.Path);
            LogContext.PushProperty("Method", context.Request.Method);
            LogContext.PushProperty("StatusCode", context.Response.StatusCode);

            return Task.CompletedTask;
        }
    }
}
```

---

## 9. EnumParse Melhorado

### Problema
`EnumParse` não trata erros adequadamente.

### Solução

**Atualizar:** `Entities/EnumParse.cs`

```csharp
namespace Biss.MultiSinkLogger.Entities
{
    public static class EnumParse
    {
        /// <summary>
        /// Converte uma string para um enum de forma segura.
        /// </summary>
        /// <typeparam name="T">Tipo do enum.</typeparam>
        /// <param name="value">Valor a ser convertido.</param>
        /// <returns>Valor do enum.</returns>
        /// <exception cref="ArgumentException">Lançada quando o valor não é válido.</exception>
        public static T ParseEnum<T>(this string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Valor não pode ser nulo ou vazio.", nameof(value));
            }

            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            var validValues = string.Join(", ", Enum.GetNames(typeof(T)));
            throw new ArgumentException(
                $"Valor '{value}' não é um {typeof(T).Name} válido. Valores válidos: {validValues}",
                nameof(value));
        }

        /// <summary>
        /// Tenta converter uma string para um enum.
        /// </summary>
        /// <typeparam name="T">Tipo do enum.</typeparam>
        /// <param name="value">Valor a ser convertido.</param>
        /// <param name="result">Resultado da conversão.</param>
        /// <returns>True se a conversão foi bem-sucedida.</returns>
        public static bool TryParseEnum<T>(this string value, out T result) where T : struct, Enum
        {
            result = default;
            return !string.IsNullOrWhiteSpace(value) && Enum.TryParse<T>(value, true, out result);
        }
    }
}
```

---

## 10. Buffer Pooling no Middleware

### Problema
Criação de novos buffers a cada requisição.

### Solução

**Atualizar:** `Middleware/LoggingMiddleware.cs`

```csharp
using System.Buffers;

namespace Biss.MultiSinkLogger.Middleware
{
    public class LoggingMiddleware
    {
        private const int MaxBodyLength = 1000;
        private static readonly ArrayPool<char> _charPool = ArrayPool<char>.Shared;

        // ... código existente ...

        private async Task<string> ReadAndTruncateAsync(Stream bodyStream)
        {
            const int maxReadSize = MaxBodyLength;
            var buffer = _charPool.Rent(maxReadSize);
            
            try
            {
                using var reader = new StreamReader(bodyStream, leaveOpen: true);
                var readSize = await reader.ReadBlockAsync(buffer, 0, maxReadSize);

                bodyStream.Position = 0;
                var bodyContent = new string(buffer, 0, readSize);

                return bodyContent.Length >= maxReadSize 
                    ? $"{bodyContent}... [truncated]" 
                    : bodyContent;
            }
            finally
            {
                _charPool.Return(buffer);
            }
        }
    }
}
```

---

## Próximos Passos

1. Implementar melhorias críticas primeiro (thread safety, validação)
2. Adicionar testes unitários para cada melhoria
3. Atualizar documentação
4. Criar testes de integração
5. Preparar release notes para versão 2.0

---

**Nota:** Estas são implementações de exemplo. Adapte conforme necessário para seu contexto específico.

