import { observer } from "mobx-react-lite";
import { useEffect, useRef, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { Container, Grid, Header, Segment, Button, Icon, Card, Label, Menu } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import ProfileEditForm from "./ProfileEditForm";
import ChangePasswordForm from "../users/ChangePasswordForm";
import { format } from "date-fns";
import UserAvatar from "../../app/common/UserAvatar";

export default observer(function ProfilePage() {
  const { userStore, projectStore, modalStore } = useStore();
  const { username } = useParams<{ username: string }>();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const uploadInFlight = useRef(false);
  const [avatarLoading, setAvatarLoading] = useState(false);

  useEffect(() => {
    if (username) {
      userStore.loadProfile(username);
      projectStore.loadUserProjects(username);
    }
  }, [userStore, projectStore, username]);

  const handleAvatarUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || uploadInFlight.current) return;
    uploadInFlight.current = true;
    setAvatarLoading(true);
    try {
      await userStore.uploadAvatar(file);
    } finally {
      uploadInFlight.current = false;
      setAvatarLoading(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleAvatarDelete = async () => {
    setAvatarLoading(true);
    try {
      await userStore.deleteAvatar();
    } finally {
      setAvatarLoading(false);
    }
  };



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
    <Container className="profile-page" style={{ marginTop: '7em', maxWidth: '1200px' }}>
      {/* Header */}
      <Segment className="profile-header-segment">
        <Grid stackable>
          <Grid.Row verticalAlign="middle">
            <Grid.Column width={10}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                <div style={{ position: 'relative', display: 'inline-block' }}>
                  <UserAvatar image={profile.image} displayName={profile.displayName} size="small" />
                  {userStore.isCurrentUser(profile.username) && (
                    <Button
                      icon="camera"
                      circular
                      size="mini"
                      loading={avatarLoading}
                      disabled={avatarLoading}
                      onClick={() => fileInputRef.current?.click()}
                      style={{ position: 'absolute', bottom: -4, right: -4, padding: '4px' }}
                    />
                  )}
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/jpeg,image/png,image/gif,image/webp"
                    style={{ display: 'none' }}
                    onChange={handleAvatarUpload}
                  />
                </div>
                <div>
                  <Header as="h2" style={{ margin: 0 }}>
                    {profile.displayName}'s Profile
                  </Header>
                  {userStore.isCurrentUser(profile.username) && profile.image && (
                    <Button
                      content="Remove photo"
                      icon="trash"
                      size="mini"
                      basic
                      color="red"
                      loading={avatarLoading}
                      disabled={avatarLoading}
                      onClick={handleAvatarDelete}
                      style={{ marginTop: '6px' }}
                    />
                  )}
                </div>
              </div>
            </Grid.Column>
            <Grid.Column width={6} textAlign="right">
              {userStore.isCurrentUser(profile.username) && (
                <Button
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

      {/* Profile Information and Quick Actions Side by Side */}
      <Grid columns={2} stackable className="profile-info-grid" style={{ marginBottom: '20px' }}>
        <Grid.Column width={10}>
          <Segment style={{ height: '100%' }}>
            <Header as="h3" dividing>
              <Icon name="user circle" />
              Profile Information
            </Header>
            <Grid stackable className="profile-details-grid">
              <Grid.Row>
                <Grid.Column width={8}>
                  <p><strong>Username:</strong> {profile.username}</p>
                  <p><strong>Display Name:</strong> {profile.displayName}</p>
                </Grid.Column>
                <Grid.Column width={8}>
                  <div>
                    <strong>Role:</strong>
                    <Label color={userStore.user?.globalRole === 'Admin' ? 'red' : userStore.user?.globalRole === 'ProjectManager' ? 'blue' : 'green'} size="large" style={{ marginLeft: '8px' }}>
                      {userStore.user?.globalRole || 'User'}
                    </Label>
                  </div>
                  <p><strong>Bio:</strong></p>
                  <div className="bio-section">
                    {profile.bio || "No bio provided yet."}
                  </div>
                </Grid.Column>
              </Grid.Row>
            </Grid>
          </Segment>
        </Grid.Column>
        <Grid.Column width={6}>
          <Segment style={{ height: '100%' }}>
            <Header as="h3" dividing>
              <Icon name="lightning" />
              Quick Actions
            </Header>
            <Menu vertical fluid style={{ width: '100%' }}>
              <Menu.Item as={Link} to="/projects">
                <Icon name="folder open" />
                View All Projects
                <Menu.Menu>
                  <Menu.Item>Browse and manage all your projects</Menu.Item>
                </Menu.Menu>
              </Menu.Item>
              <Menu.Item as={Link} to="/tickets">
                <Icon name="ticket" />
                View All Tickets
                <Menu.Menu>
                  <Menu.Item>Check your assigned and created tickets</Menu.Item>
                </Menu.Menu>
              </Menu.Item>
              {userStore.isCurrentUser(profile.username) && (
                <>
                  <Menu.Item onClick={() => modalStore.openModal(<ProfileEditForm profile={profile} />)}>
                    <Icon name="settings" />
                    Account Settings
                    <Menu.Menu>
                      <Menu.Item>Update your profile information</Menu.Item>
                    </Menu.Menu>
                  </Menu.Item>
                  <Menu.Item onClick={() => modalStore.openModal(<ChangePasswordForm />)}>
                    <Icon name="lock" />
                    Change Password
                    <Menu.Menu>
                      <Menu.Item>Update your account password</Menu.Item>
                    </Menu.Menu>
                  </Menu.Item>
                </>
              )}
            </Menu>
          </Segment>
        </Grid.Column>
      </Grid>

      {/* Projects Section */}
      <Segment>
        <Header as="h3" dividing>
          <Icon name="folder open" />
          Your Projects ({projectStore.userProjects.length})
        </Header>
        <Card.Group itemsPerRow={3} stackable>
          {projectStore.userProjects.map(project => {
            return (
              <Card key={project.id} as={Link} to={`/projects/${project.id}`} style={{ height: '200px' }}>
                <Card.Content>
                  <Card.Header>
                    {project.projectTitle}
                  </Card.Header>
                  <Card.Meta>
                    <span style={{ fontSize: '0.9em', color: '#666' }}>
                      Created {format(new Date(project.startDate!), 'MMM dd, yyyy')}
                    </span>
                  </Card.Meta>
                  <Card.Description>
                    {project.description.length > 100
                      ? `${project.description.substring(0, 100)}...`
                      : project.description}
                  </Card.Description>
                </Card.Content>
                <Card.Content extra>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div>
                      <Icon name='tasks' color="blue" />
                      <span style={{ marginLeft: '5px' }}>{project.ticketCount || 0}</span>
                      <span style={{ marginLeft: '5px', fontSize: '0.8em', color: '#666' }}>tickets</span>
                    </div>
                    <div>
                      {project.isOwner && (
                        <Icon name='star' color='yellow' />
                      )}
                    </div>
                  </div>
                </Card.Content>
              </Card>
            );
          })}
        </Card.Group>
        {projectStore.userProjects.length === 0 && (
          <Segment placeholder textAlign="center">
            <Header icon>
              <Icon name="folder open outline" />
            </Header>
            <p>No projects yet. <Link to="/createProject">Create your first project</Link> to get started!</p>
          </Segment>
        )}
      </Segment>
    </Container>
  );
});

