## Instruções de Uso do Biss.MultiSinkLogger no Cliente
![Logopq](https://github.com/bisslee/Biss.MultiSinkLogger/blob/main/img/Logopq.png)

O **Biss.MultiSinkLogger** é uma biblioteca desenvolvida para facilitar a gestão de logs em aplicações .NET. Este documento explica como configurar e usar o logger no cliente.


### Sinks Disponíveis
- **Console**: Exibe os logs no console.
- **File**: Salva os logs em arquivos.
- **CosmosDB**: Salva os logs em um banco de dados CosmosDB.
- **MongoDB**: Salva os logs em um banco de dados MongoDB.
- **MySql**: Salva os logs em um banco de dados MySQL.
- **PostgreSql**: Salva os logs em um banco de dados PostgreSQL.
- **RabbitMQ**: Envia os logs para um servidor RabbitMQ.
- **Sentry**: Envia os logs para o Sentry.
- **Slack**: Envia os logs para o Slack.
- **SqlServer**: Salva os logs em um banco de dados SQL Server.
- **Sqlite**: Salva os logs em um banco de dados SQLite.

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
      },
      ,
      {
        "Type": "SqlServer",
        "Active": false,
        "Settings": {
          "ConnectionString": "Data Source=[server],[port];Persist Security Info=True;User ID=[user];Initial Catalog=[db];Password=[pwd];TrustServerCertificate=True;",
          "TableName": "LogsTest"
        }
      },
      {
        "Type": "Sqlite",
        "Active": false,
        "Settings": {
          "ConnectionString": "c:\\Temp\\LogDB\\Logs.db",
          "TableName": "Logs"
        }
      },
      {
        "Type": "MongoDB",
        "Active": false,
        "Settings": {
          "DatabaseUrl": "mongodb://[user@uri]/Logs",
          "CollectionName": "Log",
          "CappedMaxSizeMb": 1024,
          "CappedMaxDocuments": 50000,
          "RollingInterval": "Month"
        }
      },
      {
        "Type": "CosmosDB",
        "Active": false,
        "Settings": {
          "EndpointUrl": "https://lab-logs.documents.azure.com:443/",
          "AuthorizationKey": "[key]",
          "DatabaseName": "[DbName]",
          "ContainerName": "Logzis",
          "PartitionKeyPath": "Application",
          "TimeToLive": 7
        }
      },
      {
        "Type": "PostgreSql",
        "Active": false,
        "Settings": {
          "NodeUris": ["http://localhost:9200"],
          "IndexName": "logs",
          "RollingInterval": "Day"
        }
      },
      {
      "Type": "MySql",
        "Active": false,
        "Settings": {
          "ConnectionString": "Data Source=[server],[port];Persist Security Info=True;User ID=[user];Initial Catalog=[db];Password=[pwd];TrustServerCertificate=True;",
          "TableName": "LogsTest"
        }
      },
      {
        "Type": "NewRelic",
        "Active": false,
        "Settings": {
          "EndpointUrl": ["https://log-api.newrelic.com/log/v1"],          
          "ApplicationName": "[appName]",
          "LicenseKey": "[licenseKey]",
          "InsertKey": "[insertKey]",
          "RollingInterval": "Day"
        }
      },
      {
        "Type": "RabbitMQ",
        "Active": false,
        "Settings": {
          "HosrName": "http://localhost:5341",
          "VirtualHost" = "/",
          "QueueName" = "logs",
          "UserName" = "guest",
          "Password" = "guest"
          "Port" = 5672",
          "ExchangeName" = "logs",
        }
      },
      {
        "Type": "Sentry",
        "Active": false,
        "Settings": {
          "Dsn": ["Dsn"],
          "Environment": "Production",
          "LogLevel": "Information"
        }
      },
      {
        "Type": "Slack",
        "Active": false,
        "Settings": {
          "WebhookUrl": "http://localhost:5341",
          "Channel": "#general",
          "Username": "Logger",
          "IconEmoji": ":robot_face:"
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

