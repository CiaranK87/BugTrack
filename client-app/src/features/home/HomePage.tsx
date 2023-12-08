import { Link } from "react-router-dom";
import { Button, Container, Header, Image, Segment } from "semantic-ui-react";

export default function HomePage() {
  return (
    <Segment inverted textAlign="center" vertical className="masthead">
      <Container text>
        <Header as="h1" inverted>
          <Image size="massive" src="/assets/bug-logo.png" alt="logo" style={{ marginBottom: 12 }} />
          BugTrack
        </Header>
        <Button as={Link} to="/projects" size="huge" inverted>
          Take me to projects
        </Button>
      </Container>
    </Segment>
  );
}
