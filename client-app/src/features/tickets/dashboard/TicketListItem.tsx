import { Link } from "react-router-dom";
import { Button, Card, Item, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { SyntheticEvent, useState } from "react";
import { Ticket } from "../../../app/models/ticket";

interface Props {
  ticket: Ticket;
}

export default function TicketListItem({ ticket }: Props) {
  const { ticketStore, projectStore } = useStore();
  const { deleteTicket, loading } = ticketStore;

  const [target, setTarget] = useState("");

function handleTicketDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
  setTarget(e.currentTarget.name);
  deleteTicket(id).then(() => {
    if (ticket.projectId) {
      projectStore.loadProjects();
    }
  });
}

  return (
    <Segment.Group>
      <Segment>
        <Item.Group>
          <Item>
            <Item.Content>
              <Item.Header as={Link} to={`/tickets/${ticket.id}`}>
                {ticket.title}
              </Item.Header>
              <Item.Description>Submitted by {ticket.submitter}</Item.Description>
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
      <Card.Meta>
  <span>Updated: {ticket.updated ? new Date(ticket.updated + 'Z').toUTCString().replace('GMT', '').trim().slice(0, -3) : 'Never'}</span>
</Card.Meta>
      </Segment>
      <Segment secondary>Assigned/collaborators go here</Segment>
      <Segment clearing>
        <span>{ticket.description}</span>
        <Button as={Link} to={`/tickets/${ticket.id}`} color="teal" floated="right" content="View" />
        <Button
          loading={loading && target === ticket.id}
          onClick={(e) => handleTicketDelete(e, ticket.id)}
          color="red"
          floated="right"
          content="Delete"
        />
      </Segment>
    </Segment.Group>
  );
}
