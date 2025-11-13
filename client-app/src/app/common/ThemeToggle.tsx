import React from 'react';
import { Button, Icon } from 'semantic-ui-react';
import { useTheme } from '../context/ThemeContext';

const ThemeToggle: React.FC = () => {
  const { darkMode, toggleDarkMode } = useTheme();

  return (
    <div style={{ display: 'flex', alignItems: 'center', padding: '5px 10px' }}>
      <span style={{ marginRight: '10px' }}>Dark Mode</span>
      <Button
        icon
        basic
        compact
        onClick={() => toggleDarkMode()}
        style={{
          padding: '8px',
          backgroundColor: darkMode ? 'rgba(255, 255, 255, 0.1)' : 'transparent',
          borderRadius: '4px'
        }}
        title={darkMode ? 'Switch to Light Mode' : 'Switch to Dark Mode'}
      >
        <Icon
          name={darkMode ? 'sun' : 'moon'}
          color={darkMode ? 'yellow' : 'grey'}
          size="large"
        />
      </Button>
    </div>
  );
};

export default ThemeToggle;