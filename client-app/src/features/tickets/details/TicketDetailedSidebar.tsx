import { Segment, List, Label, Item, Image, Header, Icon } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../app/stores/store";

export default observer(function TicketDetailedSidebar() {
  const { ticketStore, projectStore, userStore } = useStore();
  const { selectedTicket: ticket } = ticketStore;
  const projectParticipants = projectStore.projectParticipants.get(ticket?.projectId || '') || [];

  const submitterUser = userStore.userRegistry.get(ticket?.submitter || '');
  const assignedUser = userStore.userRegistry.get(ticket?.assigned || '');

  return (
    <>
      <Segment textAlign="center" attached="top" inverted color="teal" style={{ border: "none" }}>
        <Header>
          <Icon name="users" />
          Ticket Participants
        </Header>
      </Segment>
      <Segment attached>
        <List relaxed divided>
          <Item style={{ position: "relative" }}>
            <Label style={{ position: "absolute" }} color="orange" ribbon="right">
              Submitter
            </Label>
            <Image
              size="tiny"
              src="/assets/user.png"
              avatar
            />
            <Item.Content verticalAlign="middle">
              <Item.Header as="h4">
                <Link to={`/profile/${ticket?.submitter || ''}`}>
                  {submitterUser?.displayName || ticket?.submitter || 'Unknown'}
                </Link>
              </Item.Header>
              {submitterUser && (
                <Item.Description>
                  @{submitterUser.username}
                </Item.Description>
              )}
            </Item.Content>
          </Item>

          {ticket?.assigned && (
            <Item style={{ position: "relative" }}>
              <Label style={{ position: "absolute" }} color="blue" ribbon="right">
                Assigned
              </Label>
              <Image
                size="tiny"
                src="/assets/user.png"
                avatar
              />
              <Item.Content verticalAlign="middle">
                <Item.Header as="h4">
                  <Link to={`/profile/${ticket.assigned}`}>
                    {assignedUser?.displayName || ticket.assigned}
                  </Link>
                </Item.Header>
                {assignedUser && (
                  <Item.Description>
                    @{assignedUser.username}
                  </Item.Description>
                )}
              </Item.Content>
            </Item>
          )}

          {projectParticipants.length > 0 && (
            <Item style={{ position: "relative" }}>
              <Label style={{ position: "absolute" }} color="green" ribbon="right">
                Team
              </Label>
              <Item.Content verticalAlign="middle">
                <Item.Header as="h4">
                  {projectParticipants.length} participant{projectParticipants.length !== 1 ? 's' : ''}
                </Item.Header>
                <Item.Description>
                  Working on this project
                </Item.Description>
              </Item.Content>
            </Item>
          )}
        </List>
      </Segment>
    </>
  );
});
