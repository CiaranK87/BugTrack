#!/bin/bash

# Bash script to run E2E tests
BROWSER=${1:-"chromium"}
BASE_URL=${2:-"http://localhost:3000"}
API_URL=${3:-"https://localhost:5001"}
HEADLESS=${4:-"true"}
DEBUG=${5:-"false"}

# Set environment variables
export E2E_BASE_URL="$BASE_URL"
export E2E_API_URL="$API_URL"
export E2E_HEADLESS="$HEADLESS"

echo "=== BugTrack E2E Test Runner ==="
echo "Browser: $BROWSER"
echo "Base URL: $BASE_URL"
echo "API URL: $API_URL"
echo "Headless: $HEADLESS"
echo "Debug: $DEBUG"

# Install Playwright browsers if not already installed
echo "Installing Playwright browsers..."
dotnet tool install --global Microsoft.Playwright.CLI
playwright install

# Build test project
echo "Building test project..."
dotnet build tests/EndToEnd.Tests/EndToEnd.Tests.csproj

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

# Set test arguments
TEST_ARGS=(
    "tests/EndToEnd.Tests/EndToEnd.Tests.csproj"
    "--logger"
    "console;verbosity=detailed"
    "--"
    "ParallelizeAssemblies=true"
    "ParallelizeTestCollections=true"
    "MaxParallelThreads=4"
)

if [ "$DEBUG" = "true" ]; then
    TEST_ARGS+=("--blame")
    TEST_ARGS+=("--blame-crash")
fi

# Run tests
echo "Running E2E tests..."
dotnet test "${TEST_ARGS[@]}"

if [ $? -eq 0 ]; then
    echo "All tests passed!"
else
    echo "Some tests failed!"
    exit 1
fi