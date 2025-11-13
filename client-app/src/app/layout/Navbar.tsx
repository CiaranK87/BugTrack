import { Container, Dropdown, Image, Menu, Checkbox } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useStore } from "../stores/store";
import { observer } from "mobx-react-lite";
import { useTheme } from "../context/ThemeContext";

export default observer(function Navbar() {
  const {
    userStore: { user, logout, isAdmin },
  } = useStore();
  
  const { darkMode, toggleDarkMode } = useTheme();

  return (
    <Menu inverted fixed="top">
      <Container>
        <Menu.Item as={NavLink} to="/dashboard" header>
          <img src="/assets/bug-logo.png" alt="logo" style={{ marginRight: "10px" }} />
          BugTrack
        </Menu.Item>
        <Menu.Item as={NavLink} to="/projects" name="Projects" />
        <Menu.Item as={NavLink} to="/tickets" name="Tickets" />
        {isAdmin && (
          <Menu.Item as={NavLink} to="/admin/users" name="Admin" />
        )}
        {isAdmin && (
          <Menu.Item as={NavLink} to="/errors" name="Errors" />
        )}
        <Menu.Item position="right">
          <Image avatar spaced="right" src={"/assets/user.png"} />
          <Dropdown pointing="top left" text={`${user?.displayName} (${user?.globalRole})`}>
            <Dropdown.Menu>
              <Dropdown.Item as={Link} to={`/profile/${user?.username}`} text="My Profile" icon="user" />
              <Dropdown.Divider />
              <Dropdown.Item>
                <div
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '5px 0',
                    width: '100%'
                  }}
                  onClick={(e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    toggleDarkMode();
                  }}
                >
                  <span style={{ marginRight: '10px' }}>Dark Mode</span>
                  <Checkbox
                    toggle
                    checked={darkMode}
                    onClick={(e) => {
                      e.preventDefault();
                      e.stopPropagation();
                    }}
                    style={{ transform: 'scale(0.8)' }}
                  />
                </div>
              </Dropdown.Item>
              <Dropdown.Divider />
              <Dropdown.Item onClick={logout} text="Logout" icon="power" />
            </Dropdown.Menu>
          </Dropdown>
        </Menu.Item>
      </Container>
    </Menu>
  );
});
