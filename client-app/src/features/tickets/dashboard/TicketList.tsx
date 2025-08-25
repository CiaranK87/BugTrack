import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import TicketListItem from "./TicketListItem";
import { Button, Header } from "semantic-ui-react";
import { NavLink } from "react-router-dom";

export default observer(function TicketList() {
  const { ticketStore } = useStore();
  const { ticketsByStartDate } = ticketStore;

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Header sub color="teal" size="huge">ACTIVE TICKETS</Header>
        <Button as={NavLink} to="/createProject" basic color="teal" content="Create Project" size="small"/>
      </div>
      {ticketsByStartDate.map((ticket) => (
        <TicketListItem key={ticket.id} ticket={ticket} />
      ))}
    </>
  );
});
