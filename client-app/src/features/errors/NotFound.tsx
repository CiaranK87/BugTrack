import { Link } from "react-router-dom";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

export default function NotFound() {
  return (
    <Segment placeholder>
      <Header icon>
        <Icon name="search" />
        Opps! We couldn't find that!
      </Header>
      <Segment.Inline>
        <Button as={Link} to="/projects">
          Return to projects
        </Button>
      </Segment.Inline>
    </Segment>
  );
}
