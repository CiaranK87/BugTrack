import { Container, Dropdown, Image, Menu, Button, Icon } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useStore } from "../stores/store";
import { observer } from "mobx-react-lite";

export default observer(function Navbar() {
  const {
    userStore: { user, logout, isAdmin },
    commonStore: { darkMode, toggleDarkMode }
  } = useStore();

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
    </Menu>
  );
});
