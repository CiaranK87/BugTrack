import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Make vi available globally
Object.defineProperty(window, 'vi', {
  value: vi,
  writable: true,
});

// jsdom does not implement matchMedia (used for the dark mode default)
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});