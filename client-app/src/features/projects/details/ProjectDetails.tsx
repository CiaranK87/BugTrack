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
  const { ticketStore, projectStore } = useStore();
  const { selectedProject, loadProject, loadingInitial } = projectStore;
  const { id } = useParams<{ id: string }>();

useEffect(() => {
  if (id) {
    loadProject(id);
    ticketStore.loadTicketsByProject(id);
  }
}, [id]);

  if (loadingInitial || !selectedProject) return <LoadingComponent />;

  return (
    <Grid>
      <Grid.Column width={16}>
        <ProjectDetailedHeader project={selectedProject} />
      </Grid.Column>
      <Grid.Column width={8}>
        <ProjectDetailedInfo project={selectedProject} />
      </Grid.Column>
      <Grid.Column width={8}>
        <ProjectDetailedTicketInfo projectId={selectedProject.id} />
      </Grid.Column>
    </Grid>
  );
});
