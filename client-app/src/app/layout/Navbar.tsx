import { Button, Container, Menu } from "semantic-ui-react";

interface Props {
  openForm: () => void;
}

export default function Navbar({ openForm }: Props) {
  return (
    <Menu inverted fixed="top">
      <Container>
        <Menu.Item header>
          <img src="/assets/bug-logo.png" alt="logo" style={{ marginRight: "10px" }} />
          BugTrack
        </Menu.Item>
        <Menu.Item name="Projects" />
        <Menu.Item>
          <Button onClick={openForm} positive content="Create Project" />
        </Menu.Item>
      </Container>
    </Menu>
  );
}
