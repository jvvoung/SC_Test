param(
    [ValidateSet('win-x64', 'win-arm64')]
    [string]$Runtime = 'win-x64'
)

$ErrorActionPreference = 'Stop'
$env:AVALONIA_TELEMETRY_OPTOUT = '1'
$project = Join-Path $PSScriptRoot '..\SKCTPractice.csproj'
$output = Join-Path $PSScriptRoot "..\artifacts\$Runtime"

dotnet publish $project `
    -c Release `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $output

Write-Host "완료: $output\SKCTPractice.exe"
