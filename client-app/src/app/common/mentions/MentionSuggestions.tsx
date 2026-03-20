import React, { useState, useEffect, useRef } from 'react';
import { UserSearchDto } from '../../models/user';
import agent from '../../api/agent';
import { useTheme } from '../../context/ThemeContext';

interface Props {
  query: string;
  onSelect: (username: string) => void;
  onClose: () => void;
  position: { top: number; left: number };
  projectId?: string;
}

export default function MentionSuggestions({ query, onSelect, onClose, position, projectId }: Props) {
  const { darkMode } = useTheme();
  const [users, setUsers] = useState<UserSearchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const searchUsers = async () => {
      if (query.length >= 2) {
        setLoading(true);
        try {
          const results = await agent.Users.search(query, projectId);
          setUsers(results);
          setSelectedIndex(0);
        } catch (error) {
          console.error('Failed to search users:', error);
          setUsers([]);
        } finally {
          setLoading(false);
        }
      } else {
        setUsers([]);
      }
    };

    const timeoutId = setTimeout(searchUsers, 300);
    return () => clearTimeout(timeoutId);
  }, [query]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        onClose();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [onClose]);

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (users.length === 0) return;

    if (event.key === 'ArrowDown') {
      event.preventDefault();
      setSelectedIndex((prev) => (prev + 1) % users.length);
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      setSelectedIndex((prev) => (prev - 1 + users.length) % users.length);
    } else if (event.key === 'Enter' || event.key === 'Tab') {
      event.preventDefault();
      onSelect(users[selectedIndex].username);
    } else if (event.key === 'Escape') {
      event.preventDefault();
      onClose();
    }
  };

  if (users.length === 0 && !loading) {
    return null;
  }

  return (
    <div
      ref={containerRef}
      style={{
        position: 'absolute',
        top: `${position.top}px`,
        left: `${position.left}px`,
        backgroundColor: darkMode ? '#1e1e1e' : 'white',
        border: `1px solid ${darkMode ? '#333' : '#ddd'}`,
        borderRadius: '4px',
        boxShadow: darkMode ? '0 2px 8px rgba(0,0,0,0.5)' : '0 2px 8px rgba(0,0,0,0.15)',
        zIndex: 1000,
        maxHeight: '200px',
        overflowY: 'auto',
        minWidth: '250px',
      }}
      onKeyDown={handleKeyDown}
    >
      <div style={{
        padding: '8px',
        borderBottom: `1px solid ${darkMode ? '#333' : '#eee'}`,
        fontSize: '12px',
        fontWeight: 'bold',
        color: darkMode ? '#aaa' : '#666'
      }}>
        Select user to mention:
      </div>
      {loading ? (
        <div style={{ padding: '12px', textAlign: 'center', color: darkMode ? '#999' : '#666' }}>
          Loading...
        </div>
      ) : (
        users.map((user, index) => (
          <div
            key={user.id}
            onClick={() => onSelect(user.username)}
            style={{
              padding: '8px 12px',
              cursor: 'pointer',
              borderBottom: `1px solid ${darkMode ? '#333' : '#f5f5f5'}`,
              backgroundColor: index === selectedIndex
                ? (darkMode ? '#0d47a1' : '#f0f7ff')
                : (darkMode ? '#1e1e1e' : 'white'),
              transition: 'background-color 0.2s',
              opacity: user.isParticipant ? 1 : 0.6,
            }}
            onMouseEnter={() => setSelectedIndex(index)}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <div>
                <div style={{ fontWeight: 'bold', color: darkMode ? '#fff' : '#333' }}>
                  {user.name}
                </div>
                <div style={{ fontSize: '11px', color: darkMode ? '#aaa' : '#666' }}>
                  @{user.username}
                </div>
              </div>
              {!user.isParticipant && (
                <div style={{ 
                  fontSize: '10px', 
                  color: '#e74c3c', 
                  fontStyle: 'italic',
                  marginLeft: '8px',
                  textAlign: 'right'
                }}>
                  Not on project <br/> (Won't be notified)
                </div>
              )}
            </div>
          </div>
        ))
      )}
    </div>
  );
}
