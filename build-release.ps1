param(
    [switch]$SkipInstaller
)

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$projectFile = Join-Path $projectRoot "WordClockTaskbar.csproj"
$publishDir = Join-Path $projectRoot "bin\Release\net8.0-windows\win-x64\publish"
$publishedExe = Join-Path $publishDir "WordClockTaskbar.exe"

dotnet clean $projectFile -c Release -r win-x64 --nologo --verbosity quiet

dotnet publish $projectFile `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false

if (-not (Test-Path -LiteralPath $publishedExe)) {
    throw "Publish failed: $publishedExe was not created."
}

Write-Host "Standalone app created: $publishedExe"

if ($SkipInstaller) {
    exit 0
}

$isccCandidates = @(
    (Join-Path $env:LOCALAPPDATA "Programs\Inno Setup 6\ISCC.exe"),
    (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
    (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe")
)
$iscc = $isccCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

if (-not $iscc) {
    Write-Warning "Inno Setup 6 was not found. The standalone EXE is ready; install Inno Setup 6 to also build the setup installer."
    exit 0
}

& $iscc (Join-Path $projectRoot "installer.iss")
if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup failed with exit code $LASTEXITCODE."
}

Write-Host "Installer created in: $(Join-Path $projectRoot 'releases')"
