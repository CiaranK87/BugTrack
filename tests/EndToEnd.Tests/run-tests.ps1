# PowerShell script to run E2E tests
param(
    [string]$Browser = "chromium",
    [string]$BaseUrl = "http://localhost:3000",
    [string]$ApiUrl = "https://localhost:5001",
    [switch]$Headless = $true,
    [switch]$Debug = $false
)

# Set environment variables
$env:E2E_BASE_URL = $BaseUrl
$env:E2E_API_URL = $ApiUrl

Write-Host "=== BugTrack E2E Test Runner ===" -ForegroundColor Green
Write-Host "Browser: $Browser" -ForegroundColor Yellow
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "API URL: $ApiUrl" -ForegroundColor Yellow
Write-Host "Headless: $Headless" -ForegroundColor Yellow
Write-Host "Debug: $Debug" -ForegroundColor Yellow

# Install Playwright browsers if not already installed
Write-Host "Installing Playwright browsers..." -ForegroundColor Cyan
dotnet tool install --global Microsoft.Playwright.CLI
playwright install

# Build the test project
Write-Host "Building test project..." -ForegroundColor Cyan
dotnet build tests/EndToEnd.Tests/EndToEnd.Tests.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Set test arguments
$testArgs = @(
    "tests/EndToEnd.Tests/EndToEnd.Tests.csproj",
    "--logger",
    "console;verbosity=detailed",
    "--",
    "ParallelizeAssemblies=true",
    "ParallelizeTestCollections=true",
    "MaxParallelThreads=4"
)

if ($Debug) {
    $testArgs += "--blame"
    $testArgs += "--blame-crash"
}

if ($Headless) {
    $env:E2E_HEADLESS = "true"
} else {
    $env:E2E_HEADLESS = "false"
}

# Run the tests
Write-Host "Running E2E tests..." -ForegroundColor Cyan
dotnet test $testArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "All tests passed!" -ForegroundColor Green
} else {
    Write-Host "Some tests failed!" -ForegroundColor Red
    exit 1
}