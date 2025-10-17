import { Segment, List, Label, Item, Image, Header } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../app/stores/store";

export default observer(function TicketDetailedSidebar() {
  const { ticketStore, projectStore } = useStore();
  const { selectedTicket: ticket } = ticketStore;
  const project = ticket?.projectId ? projectStore.projectRegistry.get(ticket.projectId) : undefined;

  return (
    <>
      <Segment textAlign="center" attached="top" inverted color="teal" style={{ border: "none" }}>
        <Header>
          Ticket Participants
        </Header>
      </Segment>
      <Segment attached>
        <List relaxed divided>
          <Item style={{ position: "relative" }}>
            <Label style={{ position: "absolute" }} color="orange" ribbon="right">
              Submitter
            </Label>
            <Image size="tiny" src={"/assets/user.png"} />
            <Item.Content verticalAlign="middle">
              <Item.Header as="h3">
                <Link to={`#`}>{ticket?.submitter || 'Unknown'}</Link>
              </Item.Header>
            </Item.Content>
          </Item>

          <Item style={{ position: "relative" }}>
            <Label style={{ position: "absolute" }} color="blue" ribbon="right">
              Assigned
            </Label>
            <Image size="tiny" src={"/assets/user.png"} />
            <Item.Content verticalAlign="middle">
              <Item.Header as="h3">
                <Link to={`#`}>{ticket?.assigned || 'Unassigned'}</Link>
              </Item.Header>
            </Item.Content>
          </Item>

          <Item style={{ position: "relative" }}>
            <Label style={{ position: "absolute" }} color="green" ribbon="right">
              Project Owner
            </Label>
            <Image size="tiny" src={"/assets/user.png"} />
            <Item.Content verticalAlign="middle">
              <Item.Header as="h3">
                <Link to={`#`}>{project?.projectOwner || 'Unknown'}</Link>
              </Item.Header>
            </Item.Content>
          </Item>
        </List>
      </Segment>
    </>
  );
});
