import { observer } from "mobx-react-lite";
import { Segment, Grid, Icon, Label, Header, Divider } from "semantic-ui-react";
import { Ticket } from "../../../app/models/ticket";
import { format } from "date-fns";

interface Props {
  ticket: Ticket;
}

const getPriorityColor = (priority: string) => {
  switch (priority) {
    case 'Critical': return 'red';
    case 'High': return 'orange';
    case 'Medium': return 'yellow';
    case 'Low': return 'green';
    default: return 'grey';
  }
};

const getSeverityColor = (severity: string) => {
  switch (severity) {
    case 'Critical': return 'red';
    case 'High': return 'orange';
    case 'Medium': return 'yellow';
    case 'Low': return 'green';
    default: return 'grey';
  }
};

const getStatusColor = (status: string) => {
  switch (status) {
    case 'Open': return 'green';
    case 'In Progress': return 'blue';
    case 'Closed': return 'grey';
    default: return 'grey';
  }
};

export default observer(function TicketDetailedInfo({ ticket }: Props) {
  return (
    <Segment.Group>
      <Segment attached="top">
        <Header as="h3" dividing>
          <Icon name="info circle" color="teal" />
          Ticket Details
        </Header>
        <Grid>
          <Grid.Column width={16}>
            <p style={{ fontSize: '1.1em', lineHeight: '1.5' }}>{ticket.description}</p>
          </Grid.Column>
        </Grid>
      </Segment>
      
      <Segment attached>
        <Grid>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="flag" color="teal" />
              Priority
            </Header>
            <Label size="large" color={getPriorityColor(ticket.priority)}>
              <Icon name="flag" style={{ marginRight: '5px' }} />
              {ticket.priority}
            </Label>
          </Grid.Column>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="warning sign" color="teal" />
              Severity
            </Header>
            <Label size="large" color={getSeverityColor(ticket.severity)}>
              <Icon name="warning sign" style={{ marginRight: '5px' }} />
              {ticket.severity}
            </Label>
          </Grid.Column>
        </Grid>
      </Segment>
      
      <Segment attached>
        <Grid>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="check circle" color="teal" />
              Status
            </Header>
            <Label size="large" color={getStatusColor(ticket.status)}>
              <Icon name="circle" style={{ marginRight: '5px' }} />
              {ticket.status}
            </Label>
          </Grid.Column>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="calendar" color="teal" />
              Dates
            </Header>
            <p><strong>Created:</strong> {ticket.createdAt ? format(new Date(ticket.createdAt + 'Z'), 'MMM dd, yyyy HH:mm') : 'Never'}</p>
            <p><strong>Updated:</strong> {ticket.updated ? format(new Date(ticket.updated + 'Z'), 'MMM dd, yyyy HH:mm') : 'Never'}</p>
            {ticket.closedDate && (
              <p><strong>Closed:</strong> {format(new Date(ticket.closedDate + 'Z'), 'MMM dd, yyyy HH:mm')}</p>
            )}
          </Grid.Column>
        </Grid>
      </Segment>
      
      <Segment attached="bottom">
        <Grid>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="user" color="teal" />
              People
            </Header>
            <p><strong>Submitter:</strong> {ticket.submitter}</p>
            <p><strong>Assigned to:</strong> {ticket.assigned || 'Unassigned'}</p>
          </Grid.Column>
          <Grid.Column width={8}>
            <Header as="h4">
              <Icon name="hashtag" color="teal" />
              Ticket ID
            </Header>
            <p style={{ fontFamily: 'monospace', fontSize: '1.1em' }}>#{ticket.id}</p>
          </Grid.Column>
        </Grid>
      </Segment>
    </Segment.Group>
  );
});
