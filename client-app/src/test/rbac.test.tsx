import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import Navbar from '../app/layout/Navbar';
import { vi } from 'vitest';

// Mock the useStore hook
vi.mock('../app/stores/store', () => ({
  useStore: vi.fn(),
}));

import { useStore } from '../app/stores/store';

describe('Role-Based Access Control (RBAC)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('should show admin menu items for admin users', () => {
    // Mock the store to return an admin user
    vi.mocked(useStore).mockReturnValue({
      userStore: {
        user: { 
          displayName: 'Admin User', 
          username: 'admin',
          globalRole: 'Admin'
        },
        isAdmin: true,
        isProjectManager: false,
        logout: vi.fn(),
      },
      commonStore: {
        darkMode: false,
        toggleDarkMode: vi.fn(),
      },
    } as any);

    render(
      <BrowserRouter>
        <Navbar />
      </BrowserRouter>
    );

    // Check if admin menu items are visible
    expect(screen.getByText('Admin')).toBeInTheDocument();
    expect(screen.getByText('Errors')).toBeInTheDocument();
  });

  test('should hide admin menu items for regular users', () => {
    // Mock the store to return a regular user
    vi.mocked(useStore).mockReturnValue({
      userStore: {
        user: { 
          displayName: 'Regular User', 
          username: 'user',
          globalRole: 'User'
        },
        isAdmin: false,
        isProjectManager: false,
        logout: vi.fn(),
      },
      commonStore: {
        darkMode: false,
        toggleDarkMode: vi.fn(),
      },
    } as any);

    render(
      <BrowserRouter>
        <Navbar />
      </BrowserRouter>
    );

    // Check if admin menu items are not visible
    expect(screen.queryByText('Admin')).not.toBeInTheDocument();
    expect(screen.queryByText('Errors')).not.toBeInTheDocument();
  });

  test('should show project manager capabilities for project managers', () => {
    // Mock the store to return a project manager
    vi.mocked(useStore).mockReturnValue({
      userStore: {
        user: { 
          displayName: 'Project Manager', 
          username: 'pm',
          globalRole: 'ProjectManager'
        },
        isAdmin: false,
        isProjectManager: true,
        canCreateProjects: true,
        logout: vi.fn(),
      },
      commonStore: {
        darkMode: false,
        toggleDarkMode: vi.fn(),
      },
    } as any);

    render(
      <BrowserRouter>
        <Navbar />
      </BrowserRouter>
    );

    // Check if user info is displayed correctly
    expect(screen.getByText('Project Manager (ProjectManager)')).toBeInTheDocument();
  });

  test('should handle logout correctly', async () => {
    const mockLogout = vi.fn();
    
    // Mock the store
    vi.mocked(useStore).mockReturnValue({
      userStore: {
        user: { 
          displayName: 'Test User', 
          username: 'test',
          globalRole: 'User'
        },
        isAdmin: false,
        isProjectManager: false,
        logout: mockLogout,
      },
      commonStore: {
        darkMode: false,
        toggleDarkMode: vi.fn(),
      },
    } as any);

    render(
      <BrowserRouter>
        <Navbar />
      </BrowserRouter>
    );

    // Find and click the logout button
    const logoutButton = screen.getByText('Logout');
    logoutButton.click();

    // Check if logout was called
    expect(mockLogout).toHaveBeenCalled();
  });
});