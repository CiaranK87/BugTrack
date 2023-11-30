import { Button, Card } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default function ProjectDetails() {
  const { projectStore } = useStore();
  const { selectedProject: project, openForm, cancelSelectedProject } = projectStore;

  if (!project) return <LoadingComponent />;

  return (
    <Card fluid>
      {/* <Image src="../../../../public/assets/bug-logo.png" /> */}

      <Card.Content>
        <Card.Header>{project.name}</Card.Header>
        <span>{project.projectOwner}</span>
        <Card.Meta>
          <span>start-date: {project.startDate}</span>
        </Card.Meta>
        <Card.Meta>
          <span>end-date: {project.endDate}</span>
        </Card.Meta>
        <Card.Description>{project.description}</Card.Description>
      </Card.Content>
      <Card.Content extra>
        <Button.Group widths={2}>
          <Button onClick={() => openForm(project.id)} basic color="blue" content="Edit" />
          <Button onClick={cancelSelectedProject} basic color="grey" content="Cancel" />
        </Button.Group>
      </Card.Content>
    </Card>
  );
}
