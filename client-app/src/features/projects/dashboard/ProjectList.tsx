import { Button, Table } from "semantic-ui-react";
import { SyntheticEvent, useState } from "react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";

export default observer(function ProjectList() {
  const { projectStore } = useStore();
  const { deleteProject, projectsByStartDate, loading } = projectStore;

  const [target, setTarget] = useState("");

  function handleProjectDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
    setTarget(e.currentTarget.name);
    deleteProject(id);
  }

  return (
    <Table celled textAlign="center">
      <Table.Header>
        <Table.Row>
          <Table.HeaderCell>Project Name</Table.HeaderCell>
          <Table.HeaderCell>Project Owner</Table.HeaderCell>
          <Table.HeaderCell>Project Description</Table.HeaderCell>
          <Table.HeaderCell>Actions</Table.HeaderCell>
        </Table.Row>
      </Table.Header>

      <Table.Body>
        {projectsByStartDate.map((project) => (
          <Table.Row key={project.id}>
            <Table.Cell>{project.name}</Table.Cell>
            <Table.Cell>{project.projectOwner}</Table.Cell>
            <Table.Cell>{project.description}</Table.Cell>
            <Table.Cell>
              <Button onClick={() => projectStore.selectProject(project.id)} content="View" color="blue" />
              <Button
                name={project.id}
                loading={loading && target === project.id}
                onClick={(e) => handleProjectDelete(e, project.id)}
                content="Delete"
                color="red"
              />
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table>
  );
});

// <Segment>
//   <Item.Group divided>
//     {projects.map((project) => (
//       <Item key={project.id}>
//         <Item.Content>
//           <Item.Header as="a">{project.name}</Item.Header>
//           <Item.Meta>start date - {project.startDate}</Item.Meta>
//           <Item.Description>
//             <div>{project.description}</div>
//             <div>{project.projectOwner}</div>
//           </Item.Description>
//           <Item.Extra>
//             <Button floated="right" content="view" color="teal" />
//             <Label basic content={project.endDate} />
//           </Item.Extra>
//         </Item.Content>
//       </Item>
//     ))}
//   </Item.Group>
// </Segment>
