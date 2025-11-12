import { Link } from "react-router-dom";
import { Button, Card, Item, Segment, Confirm, Icon } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { SyntheticEvent, useState } from "react";
import { Ticket } from "../../../app/models/ticket";
import React from "react";

interface Props {
  ticket: Ticket;
}

export default function TicketListItem({ ticket }: Props) {
  const { ticketStore, projectStore } = useStore();
  const { deleteTicket, loading } = ticketStore;

  const [target, setTarget] = useState("");
  const [confirmOpen, setConfirmOpen] = useState(false);

function handleTicketDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
  e.stopPropagation();
  setTarget(e.currentTarget.name);
  setConfirmOpen(true);
}

function confirmDelete() {
  deleteTicket(target).then(() => {
    if (ticket.projectId) {
      projectStore.loadProjects();
    }
    setConfirmOpen(false);
  });
}

  return (
    <React.Fragment>
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
      <Segment clearing style={{ position: 'relative' }}>
        <span>{ticket.description}</span>
        <Button as={Link} to={`/tickets/${ticket.id}`} color="teal" floated="right" content="View" />
      </Segment>
      </Segment.Group>
      
      <Confirm
      open={confirmOpen}
      content="Are you sure you want to delete this ticket? This will move it to the deleted tickets list."
      onCancel={() => setConfirmOpen(false)}
      onConfirm={confirmDelete}
      />
    </React.Fragment>
  );
}
