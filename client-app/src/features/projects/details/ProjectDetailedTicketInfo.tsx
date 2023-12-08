import { Segment, List, Button, Table } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function ProjectDetailedInfo() {
  const { projectStore } = useStore();
  const { loading } = projectStore;

  if (loading) return <LoadingComponent />;
  return (
    <>
      <Segment textAlign="center" style={{ border: "none" }} attached="top" secondary inverted color="teal">
        Active tickets for this Project
      </Segment>
      <Segment attached>
        <List relaxed divided>
          <Table celled textAlign="center">
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell>Ticket Id</Table.HeaderCell>
                <Table.HeaderCell>Ticket Submitter</Table.HeaderCell>
                <Table.HeaderCell>Ticket Description</Table.HeaderCell>
                <Table.HeaderCell>Ticket Status</Table.HeaderCell>
                <Table.HeaderCell>View Tickets</Table.HeaderCell>
              </Table.Row>
            </Table.Header>

            <Table.Body>
              <Table.Row>
                <Table.Cell>id</Table.Cell>
                <Table.Cell>submitter</Table.Cell>
                <Table.Cell>description</Table.Cell>
                <Table.Cell>status</Table.Cell>
                <Table.Cell>
                  <Button content="View" color="blue" />
                </Table.Cell>
              </Table.Row>
            </Table.Body>
          </Table>
        </List>
      </Segment>
    </>
  );
});
