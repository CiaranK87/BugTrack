import { Grid, Header } from "semantic-ui-react";
import TicketList from "./TicketList";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import TicketFilters from "./TicketFilters";

export default observer(function TicketDashboard() {
  const { ticketStore } = useStore();
  const { loadTickets } = ticketStore;

  useEffect(() => {
    loadTickets();
  }, [loadTickets]);

  if (ticketStore.loadingInitial) return <LoadingComponent content="Loading Tickets..." />;

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
