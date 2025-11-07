import { Grid } from "semantic-ui-react";
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
    <Grid>
      <Grid.Column width={10}>
        <TicketDetailedHeader ticket={ticket} />
        <TicketDetailedInfo ticket={ticket} />
      </Grid.Column>
      <Grid.Column width={6}>
        <TicketComments ticketId={ticket.id} />
      </Grid.Column>
    </Grid>
  );
});
