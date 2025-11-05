import { Grid, Header, Segment } from "semantic-ui-react";
import TicketListItem from "./TicketListItem";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import TicketFilters from "./TicketFilters";

export default observer(function TicketDashboard() {
  const { ticketStore } = useStore();
  const { loadTickets, activeTickets, closedTickets } = ticketStore;
  const [filter, setFilter] = useState<'active' | 'closed'>('active');

  useEffect(() => {
    loadTickets();
  }, [loadTickets]);

  if (ticketStore.loadingInitial) return <LoadingComponent content="Loading Tickets..." />;

  const displayTickets = filter === 'active' ? activeTickets : closedTickets;
  const headerText = filter === 'active' ? 'ACTIVE TICKETS' : 'CLOSED TICKETS';

  return (
    <>
      <Grid>
        <Grid.Column width={10}>
          <Segment>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <Header sub color="teal" size="huge">{headerText}</Header>
            </div>
            {displayTickets.map((ticket) => (
              <TicketListItem key={ticket.id} ticket={ticket} />
            ))}
          </Segment>
        </Grid.Column>
        <Grid.Column width={6}>
          <TicketFilters
            onFilterChange={setFilter}
            currentFilter={filter}
          />
        </Grid.Column>
      </Grid>
    </>
  );
});
