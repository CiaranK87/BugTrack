# BugTrack Client App

A React + TypeScript frontend application for bug tracking and project management.

## Running Tests

This project uses Vitest and React Testing Library for testing the client-side application.


```bash
# Run all tests
npm run test

# Run tests in watch mode
npm run test:watch

# Run tests with UI
npm run test:ui

# Run tests with coverage report
npm run test:coverage
```

### Test Structure

- Component tests are located alongside their components (e.g., `Component.test.tsx`)
- Integration tests are in the `src/test/` directory
- Global test setup is in `src/test/setup.ts`

### Coverage

Coverage reports are generated in the `coverage/` directory when running `npm run test:coverage`.

### Note

For server-side testing documentation, please refer to [`../API/README.md`](../API/README.md#running-tests).

## Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
