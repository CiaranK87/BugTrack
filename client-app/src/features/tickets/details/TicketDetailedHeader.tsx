import { observer } from "mobx-react-lite";
import { Button, Header, Item, Segment, Image } from "semantic-ui-react";
import { Ticket } from "../../../app/models/ticket";
import { Link } from "react-router-dom";
import { useStore } from "../../../app/stores/store";

interface Props {
  ticket: Ticket;
}

export default observer(function TicketDetailedHeader({ ticket }: Props) {

  const { projectStore, userStore } = useStore();
  const currentUser = userStore.user;
  const project = projectStore.projectRegistry.get(ticket.projectId);

  const isSubmitter = ticket.submitter === currentUser?.username;
  const isAssigned = ticket.assigned === currentUser?.username;
  const isProjectOwner = project?.isOwner;
  const isAdmin = userStore.isAdmin;
  
  const participant = projectStore.currentProjectParticipants.find(p => p.username === currentUser?.username);
  const isProjectManager = participant?.role === "ProjectManager";

  const canManageTicket = isSubmitter || isAssigned || isProjectOwner || isProjectManager || isAdmin;

  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: "0", background: "white" }}>
        <Segment basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={ticket.title} />
                <p>Created: {ticket.createdAt ? new Date(ticket.createdAt + 'Z').toUTCString().replace('GMT', '').trim().slice(0, -3) : 'Never'}</p>
                <div style={{ display: 'flex', alignItems: 'center', gap: '15px', marginTop: '10px' }}>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <Image avatar size="mini" src="/assets/user.png" style={{ marginRight: '8px' }} />
                    <span style={{ fontSize: '0.9em', color: '#666' }}>
                      Submitted by <strong>{ticket.submitter}</strong>
                    </span>
                  </div>
                  {ticket.assigned && (
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                      <Image avatar size="mini" src="/assets/user.png" style={{ marginRight: '8px' }} />
                      <span style={{ fontSize: '0.9em', color: '#666' }}>
                        Assigned to <strong>{ticket.assigned}</strong>
                      </span>
                    </div>
                  )}
                </div>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        {canManageTicket && (
          <Button as={Link} to={`/projects/${ticket.projectId}/tickets/${ticket.id}`} color="orange" floated="right">
            Manage Ticket
          </Button>
        )}
      </Segment>
    </Segment.Group>
  );
});