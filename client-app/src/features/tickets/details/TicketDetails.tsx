import { Grid, Segment, Header, Button, Icon } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import TicketDetailedHeader from "./TicketDetailedHeader";
import TicketDetailedInfo from "./TicketDetailedInfo";
import TicketComments from "../components/TicketComments";

export default observer(function TicketDetails() {
  const { ticketStore, projectStore } = useStore();
  const { selectedTicket: ticket, loadTicket, loadingInitial, ticketError } = ticketStore;
  const { id } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    if (id) {
      loadTicket(id).then((ticket) => {
        if (ticket) {
          projectStore.loadUserRoleForProject(ticket.projectId);
          projectStore.loadProjectParticipants(ticket.projectId);
        }
      });
    }
  }, [id, loadTicket, projectStore]);

  if (loadingInitial) return <LoadingComponent />;

  if (ticketError) return (
    <Segment placeholder>
      <Header icon>
        <Icon name='ban' />
        {ticketError}
      </Header>
      <Segment.Inline>
        <Button onClick={() => navigate(-1)}>Go Back</Button>
      </Segment.Inline>
    </Segment>
  );

  if (!ticket) return <LoadingComponent />;

  return (
    <Grid stretched className="ticket-details-container">
      <Grid.Column width={16}>
        <TicketDetailedHeader ticket={ticket} />
      </Grid.Column>

      <Grid.Column width={16}>
        <TicketDetailedInfo ticket={ticket} />
      </Grid.Column>

      <Grid.Column width={16}>
        <Segment>
          <Header as="h3" dividing>
            <Header.Content>
              Discussion & Comments
            </Header.Content>
          </Header>
          <TicketComments ticketId={ticket.id} />
        </Segment>
      </Grid.Column>
    </Grid>
  );
});
