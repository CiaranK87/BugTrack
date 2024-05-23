import { observer } from "mobx-react-lite";
import { Button, Header, Item, Segment } from "semantic-ui-react";
import { Ticket } from "../../../app/models/ticket";

interface Props {
  ticket: Ticket;
}

export default observer(function TicketDetailedHeader({ ticket }: Props) {
  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: "0", background: "white" }}>
        <Segment basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={ticket.title} />
                <p>{ticket.startDate}</p>
                <p>
                  Submitted by <strong>Bob</strong>
                </p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        <Button color="teal">Collaborate</Button>
        <Button>Cancel collaboration</Button>
        <Button color="orange" floated="right">
          Manage Ticket
        </Button>
      </Segment>
    </Segment.Group>
  );
});
