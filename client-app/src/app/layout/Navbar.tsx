import { Button, Container, Menu } from "semantic-ui-react";
import { useStore } from "../stores/store";

export default function Navbar() {
  const { projectStore } = useStore();

  return (
    <Menu inverted fixed="top">
      <Container>
        <Menu.Item header>
          <img src="/assets/bug-logo.png" alt="logo" style={{ marginRight: "10px" }} />
          BugTrack
        </Menu.Item>
        <Menu.Item name="Projects" />
        <Menu.Item>
          <Button onClick={() => projectStore.openForm()} positive content="Create Project" />
        </Menu.Item>
      </Container>
    </Menu>
  );
}
