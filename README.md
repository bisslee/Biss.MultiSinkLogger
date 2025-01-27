## Biss.MultiSinkLogger - Guia para Contribuições
![Logopq](https://github.com/bisslee/Biss.MultiSinkLogger/blob/main/img/logo_trans.png)

O **Biss.MultiSinkLogger** é um componente de logging desenvolvido para unificar e padronizar os logs em projetos .NET. Este documento orienta sobre como contribuir para o projeto e criar novas versões.

---

### 1. **Visão Geral do Projeto**
O objetivo do projeto é fornecer um logger flexível, baseado no Serilog, com suporte a múltiplos sinks configuráveis.

#### Recursos:
- Configuração de múltiplos sinks (e.g., Console, Arquivo, SQL Server, MongoDB).
- Captura de logs de requisições HTTP e respostas.
- Middlewares para captura de exceções.
- Extensibilidade através de configuradores personalizados.

#### Estrutura do Repositório:
- **Biss.MultiSinkLogger:** Contém o componente principal.
- **Biss.MultiSinkLogger.API:** API de exemplo para demonstração do uso.
- **Biss.MultiSinkLogger.Tests:** Testes unitários do componente.
- **Biss.MultiSinkLogger.API.Tests:** Testes de integração para a API de exemplo.

---

### 2. **Como Contribuir**

#### a. **Configuração do Ambiente**
Certifique-se de ter as seguintes ferramentas instaladas:
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio ou VS Code
- Git

Clone o repositório:

```bash
git clone https://github.com/seuprojeto/Biss.MultiSinkLogger.git
cd Biss.MultiSinkLogger
```

Restaure os pacotes NuGet:

```bash
dotnet restore
```

#### b. **Padrões de Codificação**
- Utilize o estilo padrão da Microsoft para C#.
- Nomeie métodos e classes de forma descritiva.
- Documente códigos com XML comments.

#### c. **Execução de Testes**
Antes de enviar um PR, execute todos os testes para garantir que não haja regressões:

```bash
dotnet test
```

---

### 3. **Criando Novos Sinks**

#### Passo 1: Crie uma classe que implemente `ISinkConfigurator`
A interface `ISinkConfigurator` define os métodos necessários para configurar o novo sink.

```csharp
public class CustomSinkConfigurator : ISinkConfigurator
{
    public void Configure(LoggerConfiguration loggerConfiguration, ISinkSettings settings)
    {
        var customSettings = (CustomSinkSettings)settings;
        loggerConfiguration.WriteTo.Console();
    }
}
```

#### Passo 2: Defina as configurações do Sink

Crie uma classe que herde de `ISinkSettings` para representar as configurações do sink:

```csharp
public class CustomSinkSettings : ISinkSettings
{
    public string CustomProperty { get; set; }
}
```

#### Passo 3: Registre o Sink

Adicione o novo configurador na fábrica de sinks (`SinkConfiguratorFactory`):

```csharp
case SinkType.Custom:
    return new CustomSinkConfigurator();
```

---

### 4. **Checklist para Pull Requests**
- [ ] O código segue os padrões definidos?
- [ ] Todos os testes estão passando?
- [ ] Incluiu testes para o novo recurso ou correção?
- [ ] Atualizou a documentação, se aplicável?

---

### 5. **Roadmap do Projeto**
#### Funcionalidades Futuras:
- Integração com mais sistemas de log em nuvem.
- Dashboard para análise de logs.
- Suporte a logs distribuídos.

---

### 6. **Licença**
Este projeto está licenciado sob a [Licença MIT](https://opensource.org/licenses/MIT).

---

Agradecemos sua contribuição! Para dúvidas ou sugestões, abra uma issue no repositório.

