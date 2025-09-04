import { Segment, Button, Table, Label } from "semantic-ui-react";
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
  }, [projectId]);


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
    <>
      <Segment textAlign="center" style={{ border: "none" }} attached="top" secondary inverted color="teal">
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
        <Table celled textAlign="center">
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
      </Segment>
    </>
  );
});
