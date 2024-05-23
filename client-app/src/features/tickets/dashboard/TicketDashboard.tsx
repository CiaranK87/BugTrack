import { Grid, Header } from "semantic-ui-react";
import TicketList from "./TicketList";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import TicketFilters from "./TicketFilters";

export default observer(function TicketDashboard() {
  const { projectStore, ticketStore } = useStore();
  const { loadTickets, ticketRegistry } = ticketStore;

  useEffect(() => {
    if (ticketRegistry.size <= 0) loadTickets();
  }, [loadTickets, ticketRegistry.size]);

  if (projectStore.loadingInitial) return <LoadingComponent content="Loading App..." />;

  // if (ticketStore.loadingInitial) return <LoadingComponent content="Loading Tickets..." />;

  return (
    <>
      <Header as="h2" icon="ticket" content="All Tickets" />
      <Grid>
        <Grid.Column width="10">
          <TicketList />
        </Grid.Column>
        <Grid.Column width="6">
          <TicketFilters />
        </Grid.Column>
      </Grid>
    </>
  );
});
