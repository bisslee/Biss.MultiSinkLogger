## Instruções de Uso do Biss.MultiSinkLogger no Cliente
![Logopq](../../docs/Logopq.png)
O **Biss.MultiSinkLogger** é uma biblioteca desenvolvida para facilitar a gestão de logs em aplicações .NET. Este documento explica como configurar e usar o logger no cliente.

---

### 1. **Instalação da Biblioteca**
Adicione o Biss.MultiSinkLogger ao seu projeto via NuGet:

```bash
Install-Package Biss.MultiSinkLogger
```

---

### 2. **Configuração Inicial**

#### Adicione a configuração ao `appsettings.json`
Inclua as configurações do logger no arquivo `appsettings.json`:

```json
{
  "LoggerManagerSettings": {
    "MinimumLevel": "Warning",
    "OutputTemplate": "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}",
    "Sinks": [
      {
        "Type": "Console",
        "Active": true
      },
      {
        "Type": "File",
        "Active": true,
        "Settings": {
          "Path": "C:\\Logs\\",
          "Filename": "log.txt",
          "RollingInterval": "Day"
        }
      }
    ]
  }
}
```

#### Inicialização no `Program.cs`
No arquivo `Program.cs`, configure o logger:

```csharp
using Biss.MultiSinkLogger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Inicializa o logger com configurações do appsettings.json
LoggingManager.InitializeLogger(builder.Configuration);

// Configura o Serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Middlewares para captura de logs
app.UseExceptionLogging();
app.UseCustomLogging();

app.Run();
```

---

### 3. **Usando o Logger no Código**

#### Exemplo de Registro de Logs
Em controladores ou serviços, use o logger para registrar informações:

```csharp
using Biss.MultiSinkLogger;

public class ExampleService
{
    public void Process()
    {
        Logger.Info("Iniciando o processamento.");
        try
        {
            // Lógica de negócio
        }
        catch (Exception ex)
        {
            Logger.Error("Erro durante o processamento.", ex);
        }
    }
}
```

#### Log de Requisições HTTP
Para capturar logs de chamadas HTTP:

```csharp
using Biss.MultiSinkLogger.Http;

builder.Services.AddTransient<HttpLoggingHandler>();

builder.Services.AddHttpClient("ApiExterna")
    .AddHttpMessageHandler<HttpLoggingHandler>();
```

---

### 4. **Verificação dos Logs**
Os logs serão registrados nos sinks configurados (console, arquivos, etc.). Certifique-se de validar se os arquivos estão sendo gerados no local configurado e que as permissões de escrita estão corretas.

---

### 5. **Considerações Finais**
- Certifique-se de que os dados sensíveis não sejam registrados nos logs.
- Use variáveis de ambiente para strings de conexão em produção.

Caso tenha problemas, consulte a documentação completa ou entre em contato com o suporte.

