import React, { createContext, useContext, useEffect, ReactNode } from 'react';
import { useStore } from '../stores/store';

interface ThemeContextType {
  darkMode: boolean;
  toggleDarkMode: () => void;
  setDarkMode: (value: boolean) => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

interface ThemeProviderProps {
  children: ReactNode;
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  const { commonStore } = useStore();

  useEffect(() => {
    // Apply dark mode class to body on initial load
    if (commonStore.darkMode) {
      document.body.classList.add('dark-mode');
    }
  }, [commonStore.darkMode]);

  const contextValue: ThemeContextType = {
    darkMode: commonStore.darkMode,
    toggleDarkMode: commonStore.toggleDarkMode,
    setDarkMode: commonStore.setDarkMode,
  };

  return (
    <ThemeContext.Provider value={contextValue}>
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = (): ThemeContextType => {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};