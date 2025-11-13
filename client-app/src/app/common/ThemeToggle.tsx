import React from 'react';
import { Checkbox } from 'semantic-ui-react';
import { useTheme } from '../context/ThemeContext';

const ThemeToggle: React.FC = () => {
  const { darkMode, toggleDarkMode } = useTheme();

  return (
    <div style={{ display: 'flex', alignItems: 'center', padding: '5px 10px' }}>
      <span style={{ marginRight: '10px' }}>Dark Mode</span>
      <Checkbox
        toggle
        checked={darkMode}
        onChange={toggleDarkMode}
      />
    </div>
  );
};

export default ThemeToggle;