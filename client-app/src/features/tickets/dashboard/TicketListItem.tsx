import { Link } from "react-router-dom";
import { Button, Icon, Item, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { SyntheticEvent, useState } from "react";
import { Ticket } from "../../../app/models/ticket";
import { format } from "date-fns";

interface Props {
  ticket: Ticket;
}

export default function TicketListItem({ ticket }: Props) {
  const { ticketStore } = useStore();
  const { deleteTicket, loading } = ticketStore;

  const [target, setTarget] = useState("");

  function handleTicketDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
    setTarget(e.currentTarget.name);
    deleteTicket(id);
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
              <Item.Description>Submitted by BOB</Item.Description>
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <span>
          <Icon name="clock" /> {format(ticket.startDate!, "dd/MM/yyyy")}
        </span>
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

// <Item key={ticket.id}>
//   <Item.Content>
//     <Item.Header as="a">{ticket.title}</Item.Header>
//     <Item.Meta>{ticket.startDate}</Item.Meta>
//     <Item.Description>
//       <div>{ticket.description}</div>
//       <div>{ticket.submitter}</div>
//       <div>{ticket.updated}</div>
//     </Item.Description>
//     <Item.Extra>
//       <Button as={Link} to={`/tickets/${ticket.id}`} floated="right" content="View" color="blue" />
//       <Button
//         name={ticket.id}
//         loading={loading && target === ticket.id}
//         onClick={(e) => handleTicketDelete(e, ticket.id)}
//         floated="right"
//         content="Delete"
//         color="red"
//       />
//       <Label basic content={ticket.severity} />
//     </Item.Extra>
//   </Item.Content>
// </Item>
