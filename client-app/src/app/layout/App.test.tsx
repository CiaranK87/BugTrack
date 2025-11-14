import { render } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { createContext, ReactNode } from 'react';
import { StoreContext } from '../stores/store';
import App from './App';
import { vi } from 'vitest';

// Create a mock ThemeContext that matches the real one
const MockThemeContext = createContext({
  darkMode: false,
  toggleDarkMode: vi.fn(),
  setDarkMode: vi.fn(),
});

// Mock the ThemeContext module
vi.mock('../context/ThemeContext', () => ({
  useTheme: () => ({
    darkMode: false,
    toggleDarkMode: vi.fn(),
    setDarkMode: vi.fn(),
  }),
  ThemeProvider: ({ children }: { children: ReactNode }) => (
    <MockThemeContext.Provider value={{
      darkMode: false,
      toggleDarkMode: vi.fn(),
      setDarkMode: vi.fn(),
    }}>
      {children}
    </MockThemeContext.Provider>
  ),
}));

// Create minimal store mocks with just the properties
const mockStore = {
  commonStore: {
    token: null,
    appLoaded: true,
    setAppLoaded: vi.fn(),
    darkMode: false,
    toggleDarkMode: vi.fn(),
    setDarkMode: vi.fn(),
  },
  userStore: {
    getUser: vi.fn().mockResolvedValue(undefined),
    isLoggedIn: false,
  },
  projectStore: {
    projectRegistry: new Map(),
    selectedProject: undefined,
    loading: false,
    loadingInitial: false,
  },
  modalStore: {
    modal: {
      open: false,
      body: null,
    },
  },
  ticketStore: {
    ticketRegistry: new Map(),
    selectedTicket: undefined,
    loading: false,
    loadingInitial: false,
  },
  commentStore: {
    commentRegistry: new Map(),
    selectedComment: undefined,
    loading: false,
    loadingInitial: false,
  },
};

test('renders App component without crashing', () => {
  render(
    <StoreContext.Provider value={mockStore as any}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </StoreContext.Provider>
  );
  
  // Check if the app renders
  expect(document.body).toBeInTheDocument();
});