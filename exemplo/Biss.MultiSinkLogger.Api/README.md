## Instru��es de Uso do Biss.MultiSinkLogger no Cliente
![Logopq](../../docs/Logopq.png)
O **Biss.MultiSinkLogger** � uma biblioteca desenvolvida para facilitar a gest�o de logs em aplica��es .NET. Este documento explica como configurar e usar o logger no cliente.

---

### 1. **Instala��o da Biblioteca**
Adicione o Biss.MultiSinkLogger ao seu projeto via NuGet:

```bash
Install-Package Biss.MultiSinkLogger
```

---

### 2. **Configura��o Inicial**

#### Adicione a configura��o ao `appsettings.json`
Inclua as configura��es do logger no arquivo `appsettings.json`:

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

#### Inicializa��o no `Program.cs`
No arquivo `Program.cs`, configure o logger:

```csharp
using Biss.MultiSinkLogger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Inicializa o logger com configura��es do appsettings.json
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

### 3. **Usando o Logger no C�digo**

#### Exemplo de Registro de Logs
Em controladores ou servi�os, use o logger para registrar informa��es:

```csharp
using Biss.MultiSinkLogger;

public class ExampleService
{
    public void Process()
    {
        Logger.Info("Iniciando o processamento.");
        try
        {
            // L�gica de neg�cio
        }
        catch (Exception ex)
        {
            Logger.Error("Erro durante o processamento.", ex);
        }
    }
}
```

#### Log de Requisi��es HTTP
Para capturar logs de chamadas HTTP:

```csharp
using Biss.MultiSinkLogger.Http;

builder.Services.AddTransient<HttpLoggingHandler>();

builder.Services.AddHttpClient("ApiExterna")
    .AddHttpMessageHandler<HttpLoggingHandler>();
```

---

### 4. **Verifica��o dos Logs**
Os logs ser�o registrados nos sinks configurados (console, arquivos, etc.). Certifique-se de validar se os arquivos est�o sendo gerados no local configurado e que as permiss�es de escrita est�o corretas.

---

### 5. **Considera��es Finais**
- Certifique-se de que os dados sens�veis n�o sejam registrados nos logs.
- Use vari�veis de ambiente para strings de conex�o em produ��o.

Caso tenha problemas, consulte a documenta��o completa ou entre em contato com o suporte.

