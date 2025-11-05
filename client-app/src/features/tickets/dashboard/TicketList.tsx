import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import TicketListItem from "./TicketListItem";
import { Header } from "semantic-ui-react";

export default observer(function TicketList() {
  const { ticketStore } = useStore();
  const { activeTickets } = ticketStore;

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Header sub color="teal" size="huge">ACTIVE TICKETS</Header>
      </div>
      {activeTickets.map((ticket) => (
        <TicketListItem key={ticket.id} ticket={ticket} />
      ))}
    </>
  );
});
