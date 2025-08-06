import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { Container, Grid, Header, Segment, Button, Icon, Card } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import ProfileEditForm from "./ProfileEditForm";

export default observer(function ProfilePage() {
  const { userStore, projectStore, modalStore } = useStore();
  const { username } = useParams<{ username: string }>();

  useEffect(() => {
    if (username) {
      userStore.loadProfile(username);
      projectStore.loadUserProjects(username);
    }
  }, [userStore, projectStore, username]);

  if (userStore.loadingProfile) {
    return (
      <Container style={{ marginTop: '7em' }}>
        <Segment>
          <Icon loading name='spinner' size='big' />
          Loading profile...
        </Segment>
      </Container>
    );
  }

  if (!userStore.profile) {
    return (
      <Container style={{ marginTop: '7em' }}>
        <Segment>
          <Header as="h2">Profile not found</Header>
        </Segment>
      </Container>
    );
  }

  const { profile } = userStore;

  return (
    <Container style={{ marginTop: '7em' }}>
      <Segment>
        <Grid verticalAlign="middle">
          <Grid.Row>
            <Grid.Column width={12}>
              <Header as="h1" style={{ margin: 0 }}>
                {profile.displayName}
              </Header>
              <p><strong>@{profile.username}</strong></p>
              <div style={{ marginTop: '0.5rem' }}>
                <Icon name='folder' />
                {projectStore.userProjects.length} Projects
              </div>
            </Grid.Column>

            <Grid.Column width={4} textAlign="right">
              {userStore.isCurrentUser(profile.username) && (
                <Button
                  basic
                  content="Edit Profile"
                  icon="edit"
                  size="small"
                  onClick={() => modalStore.openModal(<ProfileEditForm profile={profile} />)}
                />
              )}
            </Grid.Column>
          </Grid.Row>
        </Grid>
      </Segment>

      <Segment>
        <Header as="h3">About</Header>
        <p>{profile.bio || "No bio provided yet."}</p>
      </Segment>

      <Segment>
        <Header as="h3">Projects</Header>
        <Card.Group centered itemsPerRow={3}>
          {projectStore.userProjects.map(project => (
            <Card key={project.id}>
              <Card.Content>
                <Card.Header>{project.projectTitle}</Card.Header>
                <Card.Meta>
                  <span>Owner: {project.projectOwner}</span>
                </Card.Meta>
                <Card.Description>
                  {project.description}
                </Card.Description>
              </Card.Content>
              <Card.Content extra>
                <Icon name='tasks' />
                {project.ticketCount || 0} tickets
                {project.isOwner && (
                  <Icon name='star' color='yellow' style={{ marginLeft: '10px' }} />
                )}
              </Card.Content>
            </Card>
          ))}
        </Card.Group>
        {projectStore.userProjects.length === 0 && (
          <p>No projects found.</p>
        )}
      </Segment>
    </Container>
  );
});
