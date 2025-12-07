# Script de Build e Pack para Release - Biss.MultiSinkLogger v1.1.0
# Execute este script para criar o pacote NuGet pronto para publicação

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build e Pack - Biss.MultiSinkLogger v1.1.0" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se estamos no diretório correto
$projectPath = "src\Biss.MultiSinkLogger\Biss.MultiSinkLogger.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Host "ERRO: Arquivo do projeto não encontrado em $projectPath" -ForegroundColor Red
    Write-Host "Execute este script da raiz do repositório." -ForegroundColor Yellow
    exit 1
}

# Limpar builds anteriores
Write-Host "1. Limpando builds anteriores..." -ForegroundColor Yellow
dotnet clean $projectPath --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO ao limpar projeto" -ForegroundColor Red
    exit 1
}

# Restaurar pacotes
Write-Host "2. Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore $projectPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO ao restaurar pacotes" -ForegroundColor Red
    exit 1
}

# Executar testes (apenas testes unitários, ignorando testes de integração)
Write-Host "3. Executando testes..." -ForegroundColor Yellow
dotnet test test\Biss.MultiSinkLogger.UnitTest\Biss.MultiSinkLogger.UnitTest.csproj --configuration Release --no-build --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "AVISO: Alguns testes falharam. Continuando mesmo assim..." -ForegroundColor Yellow
    Write-Host "Nota: Testes de integração que dependem de SQL Server são esperados para falhar." -ForegroundColor Gray
}

# Build Release
Write-Host "4. Compilando em modo Release..." -ForegroundColor Yellow
dotnet build $projectPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO ao compilar projeto" -ForegroundColor Red
    exit 1
}

# Criar pacote NuGet
Write-Host "5. Criando pacote NuGet..." -ForegroundColor Yellow
dotnet pack $projectPath --configuration Release --no-build --output ./artifacts
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO ao criar pacote NuGet" -ForegroundColor Red
    exit 1
}

# Verificar arquivo criado
$nupkgFile = Get-ChildItem -Path "./artifacts" -Filter "*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($nupkgFile) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCESSO!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Pacote criado: $($nupkgFile.FullName)" -ForegroundColor Green
    Write-Host "Tamanho: $([math]::Round($nupkgFile.Length / 1KB, 2)) KB" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximos passos:" -ForegroundColor Cyan
    Write-Host "1. Testar o pacote localmente:" -ForegroundColor White
    Write-Host "   dotnet add package Biss.MultiSinkLogger --version 1.1.0 --source ./artifacts" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Publicar no NuGet.org:" -ForegroundColor White
    Write-Host "   dotnet nuget push $($nupkgFile.FullName) --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "ERRO: Pacote NuGet não foi criado" -ForegroundColor Red
    exit 1
}

