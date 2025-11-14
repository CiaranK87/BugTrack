import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Make vi available globally
Object.defineProperty(window, 'vi', {
  value: vi,
  writable: true,
});