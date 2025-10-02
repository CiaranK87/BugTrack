import { observer } from 'mobx-react-lite';
import { Form, Dropdown, Button, Segment, Header } from 'semantic-ui-react';
import { useState, useEffect } from 'react';
import { useStore } from '../../../app/stores/store';
import { useNavigate, useParams } from 'react-router-dom';

export default observer(function ProjectAddParticipant() {
  const { projectStore, userStore } = useStore();
  const { addProjectParticipant } = projectStore;
  const { id: projectId } = useParams<{ id: string }>();
  const navigate = useNavigate();

 
  const [selectedUser, setSelectedUser] = useState<string>('');
  const [selectedRole, setSelectedRole] = useState<string>('Contributor');
  const [loading, setLoading] = useState(false);


  const [searchQuery, setSearchQuery] = useState<string>('');
  const { searchUsers, userSearchResults, loadingUsers } = userStore;


  useEffect(() => {
    if (searchQuery.trim().length >= 2) {
      searchUsers(searchQuery);
    }
  }, [searchQuery, searchUsers]);

  const handleSubmit = async () => {
    if (!selectedUser || !projectId) return;
    setLoading(true);
    try {
      await addProjectParticipant(projectId, {
        userId: selectedUser,
        role: selectedRole
      });
      navigate(`/projects/${projectId}`, { state: { message: 'Participant added successfully!' } });
    } catch (error) {
      alert('Failed to add participant');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Segment>
      <Header as="h2">Add Participant to Project</Header>
      <Form>
        <Form.Field>
          <label>Select User</label>
          <Dropdown
            placeholder="Search users..."
            fluid
            search
            selection
            loading={loadingUsers}
            options={userSearchResults.map(user => ({
              key: user.id,
              text: user.name,
              value: user.id,
            }))}
            value={selectedUser}
            onSearchChange={(_, { searchQuery }) => setSearchQuery(searchQuery)}
            onChange={(_, { value }) => setSelectedUser(value as string)}
            noResultsMessage={searchQuery.length >= 2 ? "No users found" : "Type 2+ characters"}
          />
        </Form.Field>

        <Form.Field>
          <label>Role</label>
          <Dropdown
            placeholder="Select role"
            fluid
            selection
            options={[
              { key: 'projectManager', text: 'ProjectManager', value: 'ProjectManager' },
              { key: 'developer', text: 'Developer', value: 'Developer' },
            ]}
            value={selectedRole}
            onChange={(_, { value }) => setSelectedRole(value as string)}
          />
        </Form.Field>

        <Button
          primary
          loading={loading}
          disabled={!selectedUser || loading}
          onClick={handleSubmit}
        >
          Add Participant
        </Button>
        <Button
          secondary
          onClick={() => navigate(-1)}
        >
          Cancel
        </Button>
      </Form>
    </Segment>
  );
});