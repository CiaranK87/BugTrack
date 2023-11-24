import { Button, Table } from "semantic-ui-react";
import { Project } from "../../../app/models/project";

interface Props {
  projects: Project[];
  selectProject: (id: string) => void;
  deleteProject: (id: string) => void;
}

export default function ProjectList({ projects, selectProject, deleteProject }: Props) {
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
        {projects.map((project) => (
          <Table.Row key={project.id}>
            <Table.Cell>{project.name}</Table.Cell>
            <Table.Cell>{project.projectOwner}</Table.Cell>
            <Table.Cell>{project.description}</Table.Cell>
            <Table.Cell>
              <Button onClick={() => selectProject(project.id)} content="View" color="blue" />
              <Button onClick={() => deleteProject(project.id)} content="Delete" color="red" />
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table>
  );
}

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
