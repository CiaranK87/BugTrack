import { Button, Card } from "semantic-ui-react";
import { Project } from "../../../app/models/project";

interface Props {
  project: Project;
  cancelSelectProject: () => void;
  openForm: (id: string) => void;
}

export default function ProjectDetails({ project, cancelSelectProject, openForm }: Props) {
  return (
    <Card fluid>
      {/* <Image src="../../../../public/assets/bug-logo.png" /> */}

      <Card.Content>
        <Card.Header>{project.name}</Card.Header>
        <span>{project.projectOwner}</span>
        <Card.Meta>
          <span>{project.startDate}</span>
        </Card.Meta>
        <Card.Description>{project.description}</Card.Description>
      </Card.Content>
      <Card.Content extra>
        <Button.Group widths={2}>
          <Button onClick={() => openForm(project.id)} basic color="blue" content="Edit" />
          <Button onClick={cancelSelectProject} basic color="grey" content="Cancel" />
        </Button.Group>
      </Card.Content>
    </Card>
  );
}
