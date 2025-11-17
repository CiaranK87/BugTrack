import { Link } from "react-router-dom";
import { Button, Container, Header, Image, Segment } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import LoginForm from "../users/LoginForm";
import ContactMessage from "../users/ContactMessage";

export default observer(function HomePage() {
  const { userStore, modalStore } = useStore();

  return (
    <Segment inverted textAlign="center" vertical className="masthead">
      <Container text>
        <Header as="h1" inverted>
          <Image size="massive" src="/assets/bug-logo.png" alt="logo" style={{ marginBottom: 12 }} />
          BugTrack
        </Header>
        {userStore.isLoggedIn ? (
          <>
            <Header as="h2" inverted content="Welcome" />
            <Button as={Link} to="/dashboard" size="huge" inverted>
              Go to dashboard
            </Button>
          </>
        ) : (
          <>
            <Button onClick={() => modalStore.openModal(<LoginForm />)} size="huge" inverted>
              Login
            </Button>
            <Button onClick={() => modalStore.openModal(<ContactMessage />)} size="huge" inverted>
              Request Access
            </Button>
          </>
        )}
      </Container>
    </Segment>
  );
});
