import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { useEffect } from "react";
import ProjectDetailedHeader from "./ProjectDetailedHeader";
import ProjectDetailedInfo from "./ProjectDetailedInfo";
import ProjectDetailedTicketInfo from "./ProjectDetailedTicketInfo";

export default observer(function ProjectDetails() {
  const { projectStore } = useStore();
  const { selectedProject: project, loadProject, loadingInitial } = projectStore;
  const { id } = useParams();

  useEffect(() => {
    if (id) loadProject(id);
  }, [id, loadProject]);

  if (loadingInitial || !project) return <LoadingComponent />;

  return (
    <Grid>
      <Grid.Column width={16}>
        <ProjectDetailedHeader project={project} />
      </Grid.Column>
      <Grid.Column width={8}>
        <ProjectDetailedInfo project={project} />
      </Grid.Column>
      <Grid.Column width={8}>
        <ProjectDetailedTicketInfo projectId={project.id} />
      </Grid.Column>
    </Grid>
  );
});
