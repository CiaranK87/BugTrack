import { observer } from 'mobx-react-lite';
import { useEffect, useState } from 'react';
import { Button, Header, Table, Label, Loader, Dropdown } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import { useNavigate } from 'react-router-dom';
import { ProjectParticipantDto } from '../../../app/models/project';

type ProjectParticipant = ProjectParticipantDto;

interface Props {
  projectId: string;
}

export default observer(function ProjectParticipants({ projectId }: Props) {
  const { projectStore, userStore } = useStore();
  const navigate = useNavigate();
  const { isAdmin } = userStore;

  const [editingParticipantId, setEditingParticipantId] = useState<string | null>(null);
  const [editingRole, setEditingRole] = useState<string>('');

  const {
    loadProjectParticipants,
    currentProjectParticipants,
    loadingParticipants,
    updateParticipantRole,
    removeParticipant,
    currentUserCanManage
  } = projectStore;

  useEffect(() => {
    loadProjectParticipants(projectId);
  }, [projectId, loadProjectParticipants]);

  if (loadingParticipants) return <Loader active />;

  const handleEditRole = (participant: ProjectParticipant) => {
    setEditingParticipantId(participant.userId);
    setEditingRole(participant.role);
  };

  const handleSaveRole = async (participantId: string) => {
    if (!projectId) return;

    try {
      await updateParticipantRole(projectId, participantId, { role: editingRole });
      await loadProjectParticipants(projectId);
      setEditingParticipantId(null);
    } catch (error) {
      alert('Failed to update role');
    }
  };

  const handleRemoveParticipant = async (participantId: string, displayName: string) => {
    if (!window.confirm(`Are you sure you want to remove ${displayName} from this project?`)) {
      return;
    }

    if (!projectId) return;

    try {
      await removeParticipant(projectId, participantId);
      await loadProjectParticipants(projectId);
    } catch (error) {
      alert('Failed to remove participant');
    }
  };

  return (
    <div>
      <Header as="h3">
        Project Participants
        {(currentUserCanManage || isAdmin) && (
          <Button
            primary
            floated="right"
            content="Add Participant"
            onClick={() => navigate(`/projects/${projectId}/participants/add`)}
          />
        )}
      </Header>
      
      <Table>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell>Name</Table.HeaderCell>
            <Table.HeaderCell>Username</Table.HeaderCell>
            <Table.HeaderCell>Role</Table.HeaderCell>
            <Table.HeaderCell>Actions</Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        
        <Table.Body>
          {currentProjectParticipants.map(participant => (
            <Table.Row key={participant.userId}>
              <Table.Cell>{participant.displayName}</Table.Cell>
              <Table.Cell>{participant.username}</Table.Cell>
              <Table.Cell>
                <Label color={participant.isOwner ? 'red' : 'blue'}>
                  {participant.role || 'No Role'}
                </Label>
              </Table.Cell>
              <Table.Cell>
                {editingParticipantId === participant.userId ? (
                  <>
                    <Dropdown
                      selection
                      options={[
                        { key: 'Developer', text: 'Developer', value: 'Developer' },
                        { key: 'ProjectManager', text: 'ProjectManager', value: 'ProjectManager' },
                        { key: 'User', text: 'User', value: 'User' },
                      ]}
                      value={editingRole}
                      onChange={(_, { value }) => setEditingRole(value as string)}
                      style={{ width: 140, marginRight: '8px' }}
                    />
                    <Button
                      size="mini"
                      primary
                      content="Save"
                      onClick={() => handleSaveRole(participant.userId)}
                    />
                    <Button
                      size="mini"
                      content="Cancel"
                      onClick={() => setEditingParticipantId(null)}
                    />
                  </>
                ) : (
                  <Button
                    size="mini"
                    content="Edit Role"
                    onClick={() => handleEditRole(participant)}
                    disabled={!currentUserCanManage && !isAdmin}
                  />
                )}
                <Button
                  size="mini"
                  color="red"
                  content="Remove"
                  onClick={() => handleRemoveParticipant(participant.userId, participant.displayName)}
                  disabled={!currentUserCanManage && !isAdmin}
                />
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
    </div>
  );
});