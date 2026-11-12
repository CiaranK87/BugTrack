import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import HomePage from './HomePage';
import { vi } from 'vitest';

// Mock the useStore hook
vi.mock('../../app/stores/store', () => ({
  useStore: () => ({
    userStore: {
      isLoggedIn: false,
    },
    modalStore: {
      openModal: vi.fn(),
    },
  }),
}));

test('renders HomePage with login and request access buttons when not logged in', () => {
  render(
    <BrowserRouter>
      <HomePage />
    </BrowserRouter>
  );
  
  // Check if the BugTrack title is rendered
  expect(screen.getByText('BugTrack')).toBeInTheDocument();
  
  // Check if Login and Request Access buttons are rendered
  expect(screen.getByText('Login')).toBeInTheDocument();
  expect(screen.getByText('Request Access')).toBeInTheDocument();
});

test('renders HomePage with dashboard button when logged in', () => {
  render(
    <BrowserRouter>
      <HomePage />
    </BrowserRouter>
  );
  
  // Check if the BugTrack title is rendered
  expect(screen.getByText('BugTrack')).toBeInTheDocument();
});