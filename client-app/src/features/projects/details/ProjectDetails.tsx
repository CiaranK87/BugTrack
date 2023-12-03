import { Button, Card } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { Link, useParams } from "react-router-dom";
import { useEffect } from "react";

export default observer(function ProjectDetails() {
  const { projectStore } = useStore();
  const { selectedProject: project, loadProject, loadingInitial } = projectStore;
  const { id } = useParams();

  useEffect(() => {
    if (id) loadProject(id);
  }, [id, loadProject]);

  if (loadingInitial || !project) return <LoadingComponent />;

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
          <Button as={Link} to={`/manage/${project.id}`} basic color="blue" content="Edit" />
          <Button as={Link} to="/projects" basic color="grey" content="Cancel" />
        </Button.Group>
      </Card.Content>
    </Card>
  );
});
