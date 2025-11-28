# BugTrack Test Suite

## Overview

Test suite for the BugTrack application focusing on business-critical scenarios rather than trivial property testing.

## Test Structure

### Domain Tests (`Domain.UnitTests/Models/`)

Tests core business logic and domain rules:

- **TicketTests**: Bug lifecycle workflows, status transitions, deletion patterns
- **CommentTests**: Reply chains, attachments, team collaboration scenarios
- **ProjectParticipantTests**: Role hierarchy, team composition, permission models
- **AppUserTests**: User management, role-based access, account lifecycle
- **ProjectTests**: Project management, timeline validation, team formation

### Application Tests (`Application.UnitTests/`)

Tests application services, handlers, and validation:

- **Validators**: Business rule validation for tickets and projects
- **Handlers**: Command handling with side effects and business logic
- **Factories**: Test data creation for consistent testing

### Integration Tests (`API.IntegrationTests/`)

Tests full-stack behavior including API, authentication, and data persistence:

- **TicketsControllerTests**: Complete ticket CRUD workflows through API
- **TestProgram**: Test application setup with seeded data
- **TestAuthenticationHandler**: Mock authentication for testing

### Authorization Tests (`Authorization.UnitTests/`)

Tests security and permission models:

- **ProjectRoleHandler**: Project-based permission enforcement
- **TicketAuthorizationHandler**: Ticket operation permissions
- **SimpleHandlerTests**: Basic authorization scenarios

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test tests/Domain.UnitTests/
dotnet test tests/Application.UnitTests/
dotnet test tests/API.IntegrationTests/
dotnet test tests/Authorization.UnitTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Data Strategy

- **In-memory databases**: Each test gets isolated database
- **Deterministic IDs**: Fixed GUIDs for predictable test data
- **Realistic scenarios**: Business-relevant test cases
- **Proper cleanup**: Database disposal after each test

## Key Test Scenarios

### Bug Tracking Workflows
- Creating bug reports with validation
- Status progression (Open → In Progress → Resolved)
- Assignment and notification workflows
- Soft deletion with audit trails

### Team Collaboration
- Project role hierarchy and permissions
- Comment threads and attachments
- Cross-functional team scenarios
- User access control

### API Integration
- Authentication and authorization
- End-to-end data persistence
- Error handling and validation
- Full request/response cycles

## Test Philosophy

Focus on testing business value rather than implementation details:

- Test workflows, not setters
- Validate business rules, not framework features
- Simulate real user scenarios
- Ensure data integrity and security

## Troubleshooting

### Common Issues

1. **Test Failures**: Check if test data setup matches expected business rules
2. **Database Issues**: Ensure proper cleanup in test disposal
3. **Authorization Failures**: Verify role claims and project permissions
4. **Integration Test Issues**: Check TestProgram data seeding

### Debugging Tips

- Use test names that describe the business scenario
- Include assertions that validate business outcomes
- Check both happy path and edge cases
- Verify audit trails and side effects

## Contributing

When adding new tests:

1. Focus on business scenarios, not code coverage
2. Use realistic test data and scenarios
3. Include proper setup and cleanup
4. Document business purpose in test names
5. Validate both success and failure cases

## Test Coverage Goals

- **Business Logic**: 100% coverage of critical workflows
- **API Endpoints**: All endpoints tested with authentication
- **Authorization**: All permission scenarios validated
- **Data Integrity**: All business rules enforced

---

This test suite strives for testing best practices, focused on business value and system reliability.