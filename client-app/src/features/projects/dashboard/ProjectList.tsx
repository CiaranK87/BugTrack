import { Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import ProjectListItem from "./ProjectListItem";
import { Fragment } from "react";

export default observer(function ProjectList() {
  const { projectStore } = useStore();
  const { projectsByStartDate } = projectStore;

  return (
    <>
      {projectsByStartDate.map((project) => (
        <Fragment key={project.id}>
          <Header sub color="teal">
            ACTIVE PROJECTS
          </Header>
          <ProjectListItem key={project.id} project={project} />
        </Fragment>
      ))}
    </>
  );
});
// <Segment>
//   <Item.Group divided>
//     {projects.map((project) => (
//       <Item key={project.id}>
//         <Item.Content>
//           <Item.Header as="a">{project.projectTitle}</Item.Header>
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
