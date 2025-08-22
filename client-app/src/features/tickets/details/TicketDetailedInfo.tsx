import { observer } from "mobx-react-lite";
import { Segment, Grid, Icon } from "semantic-ui-react";
import { Ticket } from "../../../app/models/ticket";

interface Props {
  ticket: Ticket;
}

export default observer(function TicketDetailedInfo({ ticket }: Props) {
  return (
    <Segment.Group>
      <Segment attached="top">
        <Grid>
          <Grid.Column width={1}>
            <Icon size="large" color="teal" name="info" />
          </Grid.Column>
          <Grid.Column width={15}>
            <p>{ticket.description}</p>
          </Grid.Column>
        </Grid>
      </Segment>
      <Segment attached>
        <Grid verticalAlign="middle">
          <Grid.Column width={1}>
            <Icon name="calendar" size="large" color="teal" />
          </Grid.Column>
          <Grid.Column width={15}>
            <p>
            <span>Updated: {ticket.updated ? new Date(ticket.updated + 'Z').toUTCString().replace('GMT', '').trim().slice(0, -3) : 'Never'}</span>
            </p>
          </Grid.Column>
        </Grid>
      </Segment>
      <Segment attached>
        <Grid verticalAlign="middle">
          <Grid.Column width={1}>
            <Icon name="exclamation" size="large" color="teal" />
          </Grid.Column>
          <Grid.Column width={11}>
            <span>{ticket.priority}</span>
          </Grid.Column>
        </Grid>
      </Segment>
    </Segment.Group>
  );
});
