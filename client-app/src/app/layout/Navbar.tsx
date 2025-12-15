import { Container, Dropdown, Image, Menu, Button, Icon } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useState } from "react";
import { useStore } from "../stores/store";
import { observer } from "mobx-react-lite";

export default observer(function Navbar() {
  const {
    userStore: { user, logout, isAdmin },
    commonStore: { darkMode, toggleDarkMode }
  } = useStore();

  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <Menu inverted fixed="top" className="navbar-menu">
      <Container className="navbar-container">
        {/* Mobile Hamburger */}
        <Menu.Item
          className="mobile-hamburger"
          onClick={() => setMobileMenuOpen(true)}
        >
          <Icon name="sidebar" size="large" style={{ margin: 0 }} />
        </Menu.Item>

        <Menu.Item as={NavLink} to="/dashboard" header className="brand-logo">
          <img src="/assets/bug-logo.png" alt="logo" style={{ marginRight: "10px" }} />
          BugTrack
        </Menu.Item>

        <Menu.Item as={NavLink} to="/projects" name="Projects" className="desktop-nav-item" />
        <Menu.Item as={NavLink} to="/tickets" name="Tickets" className="desktop-nav-item" />
        {isAdmin && (
          <Menu.Item as={NavLink} to="/admin/users" name="Admin" className="desktop-nav-item" />
        )}
        {isAdmin && (
          <Menu.Item as={NavLink} to="/errors" name="Errors" className="desktop-nav-item" />
        )}
        <Menu.Item position="right" className="user-menu-item">
          <Image avatar spaced="right" src={"/assets/user.png"} />
          <Dropdown pointing="top right" text={`${user?.displayName} (${user?.globalRole})`}>
            <Dropdown.Menu>
              <Dropdown.Item as={Link} to={`/profile/${user?.username}`} text="My Profile" icon="user" />
              <Dropdown.Divider />
              <Dropdown.Item
                onClick={(e) => {
                  e.preventDefault();
                  e.stopPropagation();
                  toggleDarkMode();
                }}
              >
                <div
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    width: '100%'
                  }}
                >
                  <span style={{ marginRight: '10px' }}>{darkMode ? 'Light Mode' : 'Dark Mode'}</span>
                  <Button
                    icon
                    basic
                    compact
                    size="mini"
                    onClick={(e) => {
                      e.preventDefault();
                      e.stopPropagation();
                      toggleDarkMode();
                    }}
                    style={{
                      padding: '5px',
                      backgroundColor: darkMode ? 'rgba(255, 255, 255, 0.1)' : 'transparent',
                      borderRadius: '4px'
                    }}
                  >
                    <Icon
                      name={darkMode ? 'sun' : 'moon'}
                      color={darkMode ? 'yellow' : 'grey'}
                    />
                  </Button>
                </div>
              </Dropdown.Item>
              <Dropdown.Divider />
              <Dropdown.Item onClick={logout} text="Logout" icon="power" />
            </Dropdown.Menu>
          </Dropdown>
        </Menu.Item>
      </Container>

      {/* Mobile Drawer */}
      <div className={`mobile-drawer ${mobileMenuOpen ? 'open' : ''}`}>
        <div className="drawer-header">
          <span className="drawer-title">Menu</span>
          <Icon name="close" size="large" onClick={() => setMobileMenuOpen(false)} style={{ cursor: 'pointer' }} />
        </div>
        <div className="drawer-content">
          <div className="drawer-section">
            <NavLink to="/dashboard" onClick={() => setMobileMenuOpen(false)} className="drawer-item">
              <Icon name="dashboard" /> Dashboard
            </NavLink>
            <NavLink to="/projects" onClick={() => setMobileMenuOpen(false)} className="drawer-item">
              <Icon name="folder" /> Projects
            </NavLink>
            <NavLink to="/tickets" onClick={() => setMobileMenuOpen(false)} className="drawer-item">
              <Icon name="ticket" /> Tickets
            </NavLink>
          </div>

          {isAdmin && (
            <div className="drawer-section">
              <div className="drawer-divider">Admin</div>
              <NavLink to="/admin/users" onClick={() => setMobileMenuOpen(false)} className="drawer-item">
                <Icon name="users" /> User Management
              </NavLink>
              <NavLink to="/errors" onClick={() => setMobileMenuOpen(false)} className="drawer-item">
                <Icon name="bug" /> Error Logs
              </NavLink>
            </div>
          )}
        </div>
      </div>

      {/* Drawer Overlay */}
      <div
        className={`drawer-overlay ${mobileMenuOpen ? 'open' : ''}`}
        onClick={() => setMobileMenuOpen(false)}
      />
    </Menu>
  );
});
