import { Header, Menu } from "semantic-ui-react";

export default function TicketFilters() {
  return (
    <>
      <Menu vertical size="large" style={{ width: "100" }}>
        <Header icon="filter" attached color="teal" content="Filters" />
        <Menu.Item content="All Tickets" />
        <Menu.Item content="All Collaborators" />
        <Menu.Item content="All Submitters" />
      </Menu>

      <Header />
    </>
  );
}
