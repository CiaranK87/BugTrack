import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import TicketListItem from "./TicketListItem";

export default observer(function TicketList() {
  const { ticketStore } = useStore();
  const { ticketsByStartDate } = ticketStore;

  return (
    <>
      {ticketsByStartDate.map((ticket) => (
        <TicketListItem key={ticket.id} ticket={ticket} />
      ))}
    </>
  );
});
