import { Header, Menu } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";

interface Props {
  onFilterChange?: (filter: 'active' | 'closed') => void;
  currentFilter?: 'active' | 'closed';
}

export default observer(function TicketFilters({ onFilterChange, currentFilter = 'active' }: Props) {
  const { ticketStore } = useStore();
  const { activeTickets, closedTickets } = ticketStore;

  return (
    <>
      <Menu vertical size="large" style={{ width: "100%" }}>
        <Header icon="filter" attached color="teal" content="Filters" />
        <Menu.Item
          content="Active Tickets"
          active={currentFilter === 'active'}
          onClick={() => onFilterChange?.('active')}
          label={{ color: 'teal', content: activeTickets.length }}
        />
        <Menu.Item
          content="Closed Tickets"
          active={currentFilter === 'closed'}
          onClick={() => onFilterChange?.('closed')}
          label={{ color: 'grey', content: closedTickets.length }}
        />

      </Menu>

      <Header />
    </>
  );
});
