import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { StoreContext } from '../app/stores/store';
import LoginForm from '../features/users/LoginForm';
import HomePage from '../features/home/HomePage';
import { vi } from 'vitest';

// Mock the agent API
vi.mock('../app/api/agent', () => ({
  default: {
    Account: {
      login: vi.fn(),
      register: vi.fn(),
      current: vi.fn(),
    },
  },
}));

// Mock the router
vi.mock('../app/router/Routes', () => ({
  router: {
    navigate: vi.fn(),
  },
}));

describe('Authentication Flow', () => {
  // Helper function to create a partial mock store
  const createMockStore = (overrides = {}) => ({
    userStore: {
      login: vi.fn().mockResolvedValue(undefined),
      register: vi.fn().mockResolvedValue(undefined),
      getUser: vi.fn().mockResolvedValue(undefined),
      user: null,
      profile: null,
      userRegistry: new Map(),
      loadingProfile: false,
      userSearchResults: [],
      loadingUsers: false,
      users: [],
      loadingUserList: false,
      updatingUserRole: false,
      get isLoggedIn() { return false; },
      get projectRoles() { return {}; },
      get isAdmin() { return false; },
      get isProjectManager() { return false; },
      get canCreateProjects() { return false; },
      logout: vi.fn(),
      searchUsers: vi.fn(),
      loadProfile: vi.fn(),
      updateProfile: vi.fn(),
      loadUsers: vi.fn(),
      updateUserRole: vi.fn(),
      updateUser: vi.fn(),
      deleteUser: vi.fn(),
      getUserById: vi.fn(),
      isCurrentUser: vi.fn(),
    },
    commonStore: {
      error: null,
      token: null,
      appLoaded: true,
      darkMode: false,
      setToken: vi.fn(),
      setAppLoaded: vi.fn(),
      setDarkMode: vi.fn(),
      toggleDarkMode: vi.fn(),
      setServerError: vi.fn(),
    },
    modalStore: {
      open: false,
      body: null,
      openModal: vi.fn(),
      closeModal: vi.fn(),
      setBody: vi.fn(),
    },
    projectStore: {
      projectRegistry: new Map(),
      deletedProjectRegistry: new Map(),
      selectedProject: undefined,
      editMode: false,
      loading: false,
      loadingInitial: false,
      userProjects: [],
      loadingUserProjects: false,
      projectRoles: {},
      projectParticipants: new Map(),
      loadingParticipants: false,
      submittingParticipant: false,
      get projectsByStartDate() { return []; },
      get deletedProjects() { return []; },
      get currentUserCanManage() { return false; },
      get currentProjectParticipants() { return []; },
      loadProjects: vi.fn(),
      loadDeletedProjects: vi.fn(),
      loadProject: vi.fn(),
      loadUserProjects: vi.fn(),
      loadUserRoleForProject: vi.fn(),
      createProject: vi.fn(),
      updateProject: vi.fn(),
      deleteProject: vi.fn(),
      adminDeleteProject: vi.fn(),
      restoreProject: vi.fn(),
      cancelProjectToggle: vi.fn(),
      clear: vi.fn(),
      loadProjectParticipants: vi.fn(),
      updateParticipants: vi.fn(),
      addProjectParticipant: vi.fn(),
      updateParticipantRole: vi.fn(),
      removeParticipant: vi.fn(),
    },
    ticketStore: {
      ticketRegistry: new Map(),
      deletedTicketRegistry: new Map(),
      selectedTicket: undefined,
      editMode: false,
      loading: false,
      loadingInitial: false,
      loadTickets: vi.fn(),
      loadDeletedTickets: vi.fn(),
      loadTicket: vi.fn(),
      createTicket: vi.fn(),
      updateTicket: vi.fn(),
      deleteTicket: vi.fn(),
      restoreTicket: vi.fn(),
      loadTicketsByProjectId: vi.fn(),
      selectTicket: vi.fn(),
      cancelSelectedTicket: vi.fn(),
      openForm: vi.fn(),
      closeForm: vi.fn(),
      clear: vi.fn(),
    },
    commentStore: {
      commentRegistry: new Map(),
      selectedComment: undefined,
      editMode: false,
      loading: false,
      loadingInitial: false,
      createComment: vi.fn(),
      updateComment: vi.fn(),
      deleteComment: vi.fn(),
      loadComments: vi.fn(),
      loadComment: vi.fn(),
      selectComment: vi.fn(),
      cancelSelectedComment: vi.fn(),
      openForm: vi.fn(),
      closeForm: vi.fn(),
      addAttachment: vi.fn(),
      deleteAttachment: vi.fn(),
      getAttachment: vi.fn(),
      clear: vi.fn(),
    },
    ...overrides
  });

  const mockStore = createMockStore();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('should display login form and allow user to log in', async () => {
    const user = userEvent.setup();
    
    render(
      <StoreContext.Provider value={mockStore as any}>
        <BrowserRouter>
          <LoginForm />
        </BrowserRouter>
      </StoreContext.Provider>
    );
    
    // Check if login form is rendered
    expect(screen.getByText('Login to BugTrack')).toBeInTheDocument();
    expect(screen.getByPlaceholderText('Email')).toBeInTheDocument();
    expect(screen.getByPlaceholderText('Password')).toBeInTheDocument();
    
    // Fill in the form
    await user.type(screen.getByPlaceholderText('Email'), 'test@example.com');
    await user.type(screen.getByPlaceholderText('Password'), 'password123');
    
    // Submit the form
    await user.click(screen.getByRole('button', { name: 'Login' }));
    
    // Check if login was called
    await waitFor(() => {
      expect(mockStore.userStore.login).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123',
        error: null,
      });
    });
  });

  test('should display error message on failed login', async () => {
    const errorMessage = 'Invalid email or password';
    const mockStoreWithError = createMockStore({
      userStore: {
        login: vi.fn().mockRejectedValue(new Error(errorMessage)),
      }
    });
    
    render(
      <StoreContext.Provider value={mockStoreWithError as any}>
        <BrowserRouter>
          <LoginForm />
        </BrowserRouter>
      </StoreContext.Provider>
    );
    
    const user = userEvent.setup();
    
    // Fill in and submit the form
    await user.type(screen.getByPlaceholderText('Email'), 'test@example.com');
    await user.type(screen.getByPlaceholderText('Password'), 'wrongpassword');
    await user.click(screen.getByRole('button', { name: 'Login' }));
    
    // Check if error message is displayed
    await waitFor(() => {
      expect(screen.getByText(errorMessage)).toBeInTheDocument();
    });
  });

  test('should show login/register buttons when user is not logged in', () => {
    render(
      <StoreContext.Provider value={mockStore as any}>
        <BrowserRouter>
          <HomePage />
        </BrowserRouter>
      </StoreContext.Provider>
    );
    
    // Check if login and register buttons are shown
    expect(screen.getByText('Login')).toBeInTheDocument();
    expect(screen.getByText('Register')).toBeInTheDocument();
    expect(screen.queryByText('Go to dashboard')).not.toBeInTheDocument();
  });

  test('should show dashboard button when user is logged in', () => {
    const loggedInStore = createMockStore({
      userStore: {
        user: { displayName: 'Test User', username: 'testuser' },
        get isLoggedIn() { return true; },
      }
    });
    
    render(
      <StoreContext.Provider value={loggedInStore as any}>
        <BrowserRouter>
          <HomePage />
        </BrowserRouter>
      </StoreContext.Provider>
    );
    
    // Check if dashboard button is shown and login/register are hidden
    expect(screen.getByText('Go to dashboard')).toBeInTheDocument();
    expect(screen.queryByText('Login')).not.toBeInTheDocument();
    expect(screen.queryByText('Register')).not.toBeInTheDocument();
  });
});