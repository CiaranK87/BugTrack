import { Grid, Segment, Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { useEffect } from "react";
import TicketDetailedHeader from "./TicketDetailedHeader";
import TicketDetailedInfo from "./TicketDetailedInfo";
import TicketComments from "../components/TicketComments";

export default observer(function TicketDetails() {
  const { ticketStore, projectStore } = useStore();
  const { selectedTicket: ticket, loadTicket, loadingInitial } = ticketStore;
  const { id } = useParams();

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

  if (loadingInitial || !ticket) return <LoadingComponent />;

  return (
    <div style={{ padding: '20px' }}>
      <Grid stackable>
        <Grid.Row>
          <Grid.Column width={16}>
            <TicketDetailedHeader ticket={ticket} />
          </Grid.Column>
        </Grid.Row>
        
        <Grid.Row>
          <Grid.Column width={16}>
            <TicketDetailedInfo ticket={ticket} />
          </Grid.Column>
        </Grid.Row>
        
        <Grid.Row>
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
        </Grid.Row>
      </Grid>
    </div>
  );
});
