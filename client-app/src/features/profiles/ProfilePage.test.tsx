import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { vi } from 'vitest';
import ProfilePage from './ProfilePage';

vi.mock('../../app/stores/store', () => ({
  useStore: vi.fn(),
}));

import { useStore } from '../../app/stores/store';

const mockStores = (globalRole: string) => {
  const user = { displayName: 'Demo User', username: 'demo', globalRole };
  vi.mocked(useStore).mockReturnValue({
    userStore: {
      user,
      isGuest: globalRole === 'Guest',
      loadingProfile: false,
      profile: { username: 'demo', displayName: 'Demo User', bio: '', image: '/api/profiles/demo/avatar' },
      loadProfile: vi.fn(),
      isCurrentUser: (username: string) => username === user.username,
      uploadAvatar: vi.fn(),
      deleteAvatar: vi.fn(),
    },
    projectStore: {
      userProjects: [],
      loadUserProjects: vi.fn(),
    },
    modalStore: {
      openModal: vi.fn(),
    },
  } as any);
};

const renderProfilePage = () =>
  render(
    <MemoryRouter initialEntries={['/profiles/demo']}>
      <Routes>
        <Route path="/profiles/:username" element={<ProfilePage />} />
      </Routes>
    </MemoryRouter>
  );

describe('ProfilePage edit controls', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('hides profile, avatar and password controls for the guest account', () => {
    mockStores('Guest');
    renderProfilePage();

    expect(screen.getByText("Demo User's Profile")).toBeInTheDocument();
    expect(screen.queryByText('Edit Profile')).not.toBeInTheDocument();
    expect(screen.queryByText('Remove photo')).not.toBeInTheDocument();
    expect(screen.queryByText('Account Settings')).not.toBeInTheDocument();
    expect(screen.queryByText('Change Password')).not.toBeInTheDocument();
  });

  test('shows profile, avatar and password controls for a regular user', () => {
    mockStores('User');
    renderProfilePage();

    expect(screen.getByText('Edit Profile')).toBeInTheDocument();
    expect(screen.getByText('Remove photo')).toBeInTheDocument();
    expect(screen.getByText('Account Settings')).toBeInTheDocument();
    expect(screen.getByText('Change Password')).toBeInTheDocument();
  });
});
