# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [1.1.1] - 2024-12-07

### 🐛 Corrigido

#### Bugs Críticos Corrigidos
- **LoggingMiddleware**: Corrigido `NullReferenceException` no segundo construtor que não inicializava `_settings`
- **SinkHealthCheck**: Removida dependência de `ILogger` via DI, agora usa `Log.Logger` diretamente para evitar falhas de resolução
- **HttpLoggingHandler**: Corrigido `NullReferenceException` ao fazer `await` em `Task<string>?` nulo - agora verifica null antes de await

### 🔧 Melhorias Técnicas

- `SinkHealthCheck` agora não requer registro de `ILogger` no DI container
- Tratamento mais robusto de conteúdo HTTP nulo em `HttpLoggingHandler`
- Inicialização segura de `LoggingMiddlewareSettings` em todos os construtores

## [1.1.0] - 2024-12-07

### 🎉 Adicionado

#### Correções Críticas
- **Thread Safety**: Implementado Double-Check Locking Pattern na inicialização do `LoggingManager` para garantir thread safety em ambientes multi-threaded
- **Validação de Configuração**: Adicionado `LoggerManagerSettingsValidator` usando `IValidateOptions` para validação na inicialização
- **Tratamento de Exceções**: `DefaultExceptionHandler` agora faz logging estruturado adequado com `LogContext`
- **Memory Leak Fix**: `LoggingMiddleware` agora usa `ArrayPool<char>` para reutilização de buffers

#### Melhorias Importantes
- **Strategy Pattern**: Refatorado sistema de sinks para usar Strategy Pattern, permitindo extensibilidade sem modificar código existente
- **Injeção de Dependência**: Implementado DI completo para todos os configuradores de sink
- **Health Checks**: Adicionado `SinkHealthCheck` para monitoramento da saúde do sistema de logging
- **Filtros de Dados Sensíveis**: Implementado `SensitiveDataFilter` para filtrar automaticamente dados sensíveis (tokens, senhas, etc.) dos logs
- **Configuração Flexível**: Criado `LoggingMiddlewareSettings` para configurar comportamento do middleware via appsettings.json

#### Melhorias de Código
- **EnumParse Melhorado**: Adicionado validação e método `TryParseEnum` para parsing seguro de enums
- **Validação de Settings**: Todos os configuradores agora validam suas configurações antes de usar
- **Documentação XML**: Melhorada documentação XML em classes e métodos principais

#### Testes
- **56 novos testes unitários** cobrindo todas as melhorias implementadas
- Cobertura completa de thread safety, validação, filtros, health checks e configuradores

### 🔄 Alterado

- **ISinkConfigurator**: Adicionadas propriedades `SupportedSinkType` e método `ValidateSettings()`
- **SinkConfiguratorFactory**: Agora usa DI, mantendo método estático para compatibilidade (deprecated)
- **LoggingMiddleware**: Aceita `IOptions<LoggingMiddlewareSettings>` para configuração
- **HttpLoggingHandler**: Agora filtra dados sensíveis automaticamente
- **DefaultExceptionHandler**: Implementado logging estruturado completo

### 🔒 Segurança

- Filtros automáticos para headers sensíveis (Authorization, API keys, etc.)
- Filtros para padrões em body/query string (passwords, tokens, etc.)
- Sanitização de connection strings antes de logar

### ⚠️ Deprecated

- `SinkConfiguratorFactory.GetConfiguratorStatic()` - Use dependency injection instead

### 📝 Documentação

- Criada análise arquitetural completa
- Documentação de correções críticas
- Documentação de melhorias da Fase 2
- Guia de implementação de melhorias
- Documentação completa de testes unitários

---

## [1.0.1] - Data anterior

### Adicionado
- Versão inicial do pacote
- Suporte a múltiplos sinks (Console, File, SQL Server, MongoDB, etc.)
- Middlewares para logging HTTP e exceções
- Extensibilidade através de configuradores

---

## Links

- [Análise Arquitetural](ANALISE_ARQUITETURAL.md)
- [Correções Críticas](CORRECOES_CRITICAS.md)
- [Melhorias Fase 2](MELHORIAS_FASE2.md)
- [Testes Unitários](TESTES_UNITARIOS.md)

