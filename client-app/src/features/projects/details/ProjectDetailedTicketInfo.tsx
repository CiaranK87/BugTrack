import { Segment, Button, Table, Label, Icon, Header } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Link } from "react-router-dom";
import { useEffect } from "react";

interface Props {
  projectId: string;
}

export default observer(function ProjectDetailedTicketInfo({ projectId }: Props) {
  const { ticketStore } = useStore();
  const { loadTicketsByProject, getProjectTickets, loadingInitial } = ticketStore;

  const tickets = getProjectTickets(projectId);

  useEffect(() => {
    loadTicketsByProject(projectId);
  }, [projectId, loadTicketsByProject]);


  const getPriorityColor = (priority: string) => {
    switch (priority?.toLowerCase()) {
      case 'high':
      case 'urgent':
        return 'red';
      case 'medium':
        return 'yellow';
      case 'low':
        return 'green';
      default:
        return 'grey';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status?.toLowerCase()) {
      case 'open':
      case 'new':
        return 'blue';
      case 'in progress':
      case 'active':
        return 'yellow';
      case 'resolved':
      case 'completed':
        return 'green';
      case 'closed':
        return 'grey';
      default:
        return 'grey';
    }
  };

  if (loadingInitial) return <LoadingComponent />;

  return (
    <div className="project-ticket-info-container">
      <Segment textAlign="center" style={{ border: "none" }} attached="top" secondary inverted color="teal" className="project-ticket-header">
        Active tickets for this Project
      </Segment>
      <Segment attached>
        <Button
          as={Link}
          to={`/projects/${projectId}/tickets/create`}
          color="teal"
          content="Create Ticket"
          style={{ marginBottom: '1em' }}
        />
        <Table celled textAlign="center" className="project-ticket-table-desktop">
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell>Title</Table.HeaderCell>
              <Table.HeaderCell>Priority</Table.HeaderCell>
              <Table.HeaderCell>Status</Table.HeaderCell>
              <Table.HeaderCell>Assigned</Table.HeaderCell>
              <Table.HeaderCell>Due Date</Table.HeaderCell>
              <Table.HeaderCell>Actions</Table.HeaderCell>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {tickets.map(ticket => (
              <Table.Row key={ticket.id}>
                <Table.Cell>{ticket.title}</Table.Cell>
                <Table.Cell>
                  <Label color={getPriorityColor(ticket.priority)} size="mini">
                    {ticket.priority}
                  </Label>
                </Table.Cell>
                <Table.Cell>
                  <Label color={getStatusColor(ticket.status)} size="mini">
                    {ticket.status}
                  </Label>
                </Table.Cell>
                <Table.Cell>{ticket.assigned}</Table.Cell>
                <Table.Cell>{ticket.endDate ? new Date(ticket.endDate).toLocaleDateString() : 'No due date'}</Table.Cell>
                <Table.Cell>
                  <Button as={Link} to={`/tickets/${ticket.id}`} content="View" color="blue" size="mini" />
                </Table.Cell>
              </Table.Row>
            ))}
          </Table.Body>
        </Table>

        {/* Mobile Card View */}
        <div className="project-ticket-cards-mobile">
          {tickets.map(ticket => (
            <div key={ticket.id} className="project-ticket-mobile-card">
              <div className="ticket-card-header">
                <Header as="h4">
                  <Icon name="ticket" />
                  <Header.Content>
                    {ticket.title}
                    <Header.Subheader>{ticket.assigned || 'Unassigned'}</Header.Subheader>
                  </Header.Content>
                </Header>
                <Label color={getStatusColor(ticket.status)} size="small" className="ticket-status-label">
                  {ticket.status}
                </Label>
              </div>

              <div className="ticket-card-content">
                <div className="ticket-detail-item">
                  <span className="detail-label">Priority:</span>
                  <Label color={getPriorityColor(ticket.priority)} size="mini" className="detail-value">{ticket.priority}</Label>
                </div>
                <div className="ticket-detail-item">
                  <span className="detail-label">Due Date:</span>
                  <span className="detail-value">{ticket.endDate ? new Date(ticket.endDate).toLocaleDateString() : 'No due date'}</span>
                </div>
              </div>

              <div className="ticket-card-actions">
                <Button as={Link} to={`/tickets/${ticket.id}`} content="View Ticket" color="blue" fluid size="small" icon="eye" />
              </div>
            </div>
          ))}
          {tickets.length === 0 && (
            <Segment basic textAlign="center">
              <p>No active tickets found for this project.</p>
            </Segment>
          )}
        </div>
      </Segment>
    </div>
  );
});
