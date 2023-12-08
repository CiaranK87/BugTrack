import Calendar from "react-calendar";
import { Header, Menu } from "semantic-ui-react";

export default function ProjectFilters() {
  return (
    <>
      <Menu vertical size="large" style={{ width: "100%" }}>
        <Header icon="filter" attached color="teal" content="filters" />
        <Menu.Item content="All Projects" />
        <Menu.Item content="Filter 2" />
        <Menu.Item content="Filter 3" />
      </Menu>
      <Header />
      <Calendar />
    </>
  );
}
