import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import LoginForm from './LoginForm';
import { vi } from 'vitest';

// Mock the useStore hook
vi.mock('../../app/stores/store', () => ({
  useStore: () => ({
    userStore: {
      login: vi.fn().mockResolvedValue(undefined),
    },
  }),
}));

test('renders LoginForm with required fields', () => {
  render(<LoginForm />);
  
  // Check if the header is rendered
  expect(screen.getByText('Login to BugTrack')).toBeInTheDocument();
  
  // Check if the input fields are rendered
  expect(screen.getByPlaceholderText('Email')).toBeInTheDocument();
  expect(screen.getByPlaceholderText('Password')).toBeInTheDocument();
  
  // Check if the login button is rendered
  expect(screen.getByRole('button', { name: 'Login' })).toBeInTheDocument();
});

test('submits form with email and password', async () => {
  const mockLogin = vi.fn().mockResolvedValue(undefined);
  vi.doMock('../../app/stores/store', () => ({
    useStore: () => ({
      userStore: {
        login: mockLogin,
      },
    }),
  }));
  
  const user = userEvent.setup();
  render(<LoginForm />);
  
  // Fill in the form
  await user.type(screen.getByPlaceholderText('Email'), 'test@example.com');
  await user.type(screen.getByPlaceholderText('Password'), 'password123');
  
  // Submit the form
  await user.click(screen.getByRole('button', { name: 'Login' }));
  
});