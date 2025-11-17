import { observer } from "mobx-react-lite";
import { Button, Header, Item, Segment, Image, Confirm } from "semantic-ui-react";
import { Ticket } from "../../../app/models/ticket";
import { Link, useNavigate } from "react-router-dom";
import { useStore } from "../../../app/stores/store";
import { useState } from "react";
import { logger } from "../../../app/utils/logger";

interface Props {
  ticket: Ticket;
}

export default observer(function TicketDetailedHeader({ ticket }: Props) {
  const navigate = useNavigate();
  const { projectStore, userStore, ticketStore } = useStore();
  const currentUser = userStore.user;
  const [isClosing, setIsClosing] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  
  const project = ticket.project || projectStore.projectRegistry.get(ticket.projectId);

  const isSubmitter = ticket.submitter === currentUser?.username;
  const isAssigned = ticket.assigned === currentUser?.username;
  const isProjectOwner = project?.isOwner;
  const isAdmin = userStore.isAdmin;
  
  const projectParticipants = projectStore.projectParticipants.get(ticket.projectId) || [];
  const participant = projectParticipants.find(p => p.username === currentUser?.username);
  const isProjectManager = participant?.role === "ProjectManager";
  const isDeveloper = participant?.role === "Developer";

  const canManageTicket = isSubmitter || isAssigned || isProjectOwner || isProjectManager || isDeveloper || isAdmin;
  
  const canCloseTicket = isAdmin || isProjectManager || isDeveloper;
  const [showConfirm, setShowConfirm] = useState(false);
  const [showReopenConfirm, setShowReopenConfirm] = useState(false);

  const handleMarkAsClosed = async () => {
    setIsClosing(true);
    try {
      const updatedTicket = {
        ...ticket,
        status: "Closed",
        closedDate: new Date()
      };
      await ticketStore.updateTicket(updatedTicket);
      ticketStore.loadTicketsByProject(ticket.projectId);
    } catch (error) {
      logger.error("Failed to mark ticket as closed", error);
    } finally {
      setIsClosing(false);
      setShowConfirm(false);
    }
  };

  const handleReopenTicket = async () => {
    setIsClosing(true);
    try {
      const updatedTicket = {
        ...ticket,
        status: "Open",
        closedDate: null
      };
      await ticketStore.updateTicket(updatedTicket);
      ticketStore.loadTicketsByProject(ticket.projectId);
    } catch (error) {
      logger.error("Failed to reopen ticket", error);
    } finally {
      setIsClosing(false);
      setShowReopenConfirm(false);
    }
  };

  const handleConfirmClose = () => {
    setShowConfirm(true);
  };

  const handleConfirmReopen = () => {
    setShowReopenConfirm(true);
  };

  const handleDeleteTicket = async () => {
    try {
      await ticketStore.deleteTicket(ticket.id);
      navigate('/tickets');
    } catch (error) {
      logger.error("Failed to delete ticket", error);
    }
  };

  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: "0", background: "white" }}>
        <Segment basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Header size="huge" content={ticket.title} />
                  {(project || ticket.projectTitle) && (
                    <span
                      style={{
                        fontSize: '0.9em',
                        color: '#666',
                        cursor: 'pointer'
                      }}
                      onClick={() => navigate(`/projects/${ticket.projectId}`)}
                    >
                      Project - <strong>{project?.projectTitle || ticket.projectTitle}</strong>
                    </span>
                  )}
                </div>
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
      <Segment clearing attached="bottom" style={{ position: 'relative' }}>
        {canManageTicket && (
          <>
            {ticket.status !== "Closed" ? (
              canCloseTicket && (
                <Button
                  color="green"
                  floated="right"
                  loading={isClosing}
                  onClick={handleConfirmClose}
                  content="Mark as Closed"
                />
              )
            ) : (
              canCloseTicket && (
                <Button
                  color="blue"
                  floated="right"
                  loading={isClosing}
                  onClick={handleConfirmReopen}
                  content="Reopen Ticket"
                />
              )
            )}
           
            <Confirm
              open={showConfirm}
              content="Are you sure you want to mark this ticket as closed? You can reopen it later if needed."
              onCancel={() => setShowConfirm(false)}
              onConfirm={handleMarkAsClosed}
              confirmButton="Yes, close it"
              cancelButton="Cancel"
              size="small"
            />
           
            <Confirm
              open={showReopenConfirm}
              content="Are you sure you want to reopen this ticket? It will be set to 'Open' status."
              onCancel={() => setShowReopenConfirm(false)}
              onConfirm={handleReopenTicket}
              confirmButton="Yes, reopen it"
              cancelButton="Cancel"
              size="small"
            />
           
            <Confirm
              open={showDeleteConfirm}
              content="Are you sure you want to delete this ticket? This action can be reversed by an administrator."
              onCancel={() => setShowDeleteConfirm(false)}
              onConfirm={handleDeleteTicket}
              confirmButton="Yes, delete it"
              cancelButton="Cancel"
              size="small"
            />
            <Button as={Link} to={`/projects/${ticket.projectId}/tickets/${ticket.id}`} color="orange" floated="right">
              Manage Ticket
            </Button>
          </>
        )}
      </Segment>
    </Segment.Group>
  );
});