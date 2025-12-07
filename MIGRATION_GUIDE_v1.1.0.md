# Guia de Migração - v1.0.1 para v1.1.0

## 📋 Visão Geral

A versão **1.1.0** é **100% backward compatible** com a v1.0.1. Não há breaking changes, mas há novas funcionalidades disponíveis que você pode optar por usar.

---

## ✅ Compatibilidade

### Sem Breaking Changes
- ✅ Todas as APIs existentes continuam funcionando
- ✅ Configuração existente continua válida
- ✅ Código existente não precisa ser alterado

### Novas Funcionalidades (Opcionais)
- ✅ Validação de configuração na inicialização
- ✅ Health checks para monitoramento
- ✅ Filtros automáticos de dados sensíveis
- ✅ Configuração flexível do middleware

---

## 🚀 Migração Passo a Passo

### Passo 1: Atualizar Pacote

```bash
dotnet add package Biss.MultiSinkLogger --version 1.1.0
```

Ou via NuGet Package Manager:
```
Update-Package Biss.MultiSinkLogger -Version 1.1.0
```

### Passo 2: Verificar Compilação

```bash
dotnet build
```

**Resultado esperado:** ✅ Compila sem erros

### Passo 3: Executar Aplicação

**Resultado esperado:** ✅ Funciona exatamente como antes

---

## 🎯 Usando Novas Funcionalidades (Opcional)

### 1. Validação de Configuração

**Antes (v1.0.1):**
```csharp
services.AddCustomLogging();
```

**Agora (v1.1.0) - Recomendado:**
```csharp
services.AddCustomLogging(configuration);
```

**Benefício:** Validação automática na inicialização, detecta erros antes de executar.

### 2. Health Checks

**Novo (v1.1.0):**
```csharp
// Já registrado automaticamente ao usar AddCustomLogging(configuration)
app.MapHealthChecks("/health");
```

**Acesso:**
```
GET /health
GET /health/multi_sink_logger
```

### 3. Filtros de Dados Sensíveis

**Automático (v1.1.0):**
Os filtros são aplicados automaticamente no `LoggingMiddleware` e `HttpLoggingHandler`.

**Configuração (Opcional):**
```json
{
  "LoggingMiddleware": {
    "FilterSensitiveData": true,
    "MaxBodyLength": 2000
  }
}
```

### 4. Configuração do Middleware

**Novo (v1.1.0):**
```json
{
  "LoggingMiddleware": {
    "MaxBodyLength": 2000,
    "FilterSensitiveData": true
  }
}
```

---

## ⚠️ Avisos e Deprecations

### Método Deprecated

**SinkConfiguratorFactory.GetConfiguratorStatic()**

**Antes:**
```csharp
var configurator = SinkConfiguratorFactory.GetConfiguratorStatic(SinkType.Console);
```

**Agora (Recomendado):**
```csharp
// Use DI
services.AddCustomLogging(configuration);
// Injete SinkConfiguratorFactory onde necessário
```

**Nota:** O método estático ainda funciona, mas será removido em versão futura.

---

## 🔍 Verificações Pós-Migração

### Checklist

- [ ] Aplicação compila sem erros
- [ ] Aplicação inicia corretamente
- [ ] Logs estão sendo gerados normalmente
- [ ] Sinks configurados funcionam
- [ ] Health checks disponíveis (se usando)
- [ ] Dados sensíveis estão sendo filtrados (verificar logs)

---

## 📊 Melhorias Automáticas

Mesmo sem alterar código, você já se beneficia de:

1. ✅ **Thread Safety** - Inicialização thread-safe garantida
2. ✅ **Validação** - Se usar `AddCustomLogging(configuration)`, validação automática
3. ✅ **Performance** - Uso de ArrayPool reduz alocações de memória
4. ✅ **Segurança** - Filtros automáticos de dados sensíveis (se habilitado)
5. ✅ **Tratamento de Exceções** - Logging estruturado melhorado

---

## 🆘 Problemas Conhecidos

### Nenhum Problema Conhecido

A versão 1.1.0 foi testada extensivamente e é totalmente compatível com 1.0.1.

---

## 📚 Recursos Adicionais

- [Análise Arquitetural](ANALISE_ARQUITETURAL.md)
- [Correções Críticas](CORRECOES_CRITICAS.md)
- [Melhorias Fase 2](MELHORIAS_FASE2.md)
- [Testes Unitários](TESTES_UNITARIOS.md)
- [CHANGELOG](CHANGELOG.md)

---

## 💬 Suporte

Se encontrar problemas durante a migração:

1. Verifique o [CHANGELOG](CHANGELOG.md) para mudanças detalhadas
2. Consulte a [documentação de análise](ANALISE_ARQUITETURAL.md)
3. Abra uma issue no repositório GitHub

---

**Versão:** 1.1.0  
**Data:** 2024  
**Status:** ✅ **100% Backward Compatible**

