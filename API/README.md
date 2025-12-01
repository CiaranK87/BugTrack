# BugTrack API

This is the backend API for the BugTrack application, built with .NET 7.

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) - (version 7)

### Setup
1. Navigate to the API project folder
   ```bash
   cd API
   ```

2. Restore project dependencies
   ```bash
   dotnet restore
   ```

3. Set up the database
   ```bash
   dotnet ef database update
   ```

4. Build the project
   ```bash
   dotnet build
   ```

5. Run the API server
   ```bash
   dotnet run
   ```

Visit http://localhost:5000 in your browser to see if the API is running.

## Running Tests

This project includes a comprehensive test suite covering all layers of the backend application:

### Test Structure
- **Unit Tests**: Test individual components in isolation
  - `Domain.UnitTests`: Tests for domain models and business logic (51 tests)
  - `Application.UnitTests`: Tests for application services and handlers (61 tests)
  - `Authorization.UnitTests`: Tests for security and authorization logic (53 tests)
- **Integration Tests**: Test API endpoints with real database interactions
  - `API.IntegrationTests`: Tests for API controllers and data flow (11 tests)
- **End-to-End Tests**: Test complete user workflows through browser
  - `EndToEnd.Tests`: Tests for UI interactions and full application flow (16 tests)

### Running Tests

#### All Tests
```bash
cd tests
dotnet test
```

#### Specific Test Categories
```bash
# Unit tests only
cd tests
dotnet test --filter "Category!=Integration && Category!=EndToEnd"

# Integration tests only
cd tests
dotnet test API.IntegrationTests

# End-to-end tests (requires API to be running)
cd tests
# First start the API:
cd ../API
dotnet run

# Then in another terminal, run E2E tests:
cd ../tests
cmd /c "set RUN_E2E_TESTS=true&& dotnet test EndToEnd.Tests"
```

#### E2E Test Prerequisites
- Both API and client-app should be running
- Playwright browsers automatically installed by test runner
- Test users should exist in the system (admin@test.com, user@test.com, pm@test.com)

#### E2E Test Configuration
The tests use environment variables for configuration:
- `E2E_BASE_URL`: Frontend URL (default: http://localhost:3000)
- `E2E_API_URL`: API URL (default: http://localhost:5000)
- `RUN_E2E_TESTS`: Must be set to true to run E2E tests

### Test Data
All test data is aligned with production application values:
- **Priority Levels**: Low, Medium, High, Critical
- **Severity Levels**: Low, Medium, High, Critical  
- **Ticket Status**: Open, In Progress, Closed
- **User Roles**: Owner, Admin, ProjectManager, Developer, User

### Test Coverage
- **Total Tests**: 192 tests
- **Coverage Areas**:
  - Domain entities and business rules
  - Application services and command handlers
  - Authorization and security policies
  - API endpoints and data validation
  - User interface workflows and interactions

### Test Technologies
- **xUnit**: Test framework
- **FluentAssertions**: Assertion library
- **Moq**: Mocking framework
- **Playwright**: End-to-end browser automation
- **Microsoft.EntityFrameworkCore.InMemory**: Database testing