import { Grid, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { useEffect } from "react";
import ProjectDetailedHeader from "./ProjectDetailedHeader";
import ProjectDetailedTicketInfo from "./ProjectDetailedTicketInfo";
import ProjectParticipants from "./ProjectParticipants";

export default observer(function ProjectDetails() {
  const { projectStore } = useStore();
  const { selectedProject, loadProject, loadingInitial } = projectStore;
  const { id } = useParams<{ id: string }>();

  useEffect(() => {
    if (id) {
      loadProject(id);
    }
  }, [id, loadProject]);

  if (loadingInitial || !selectedProject) return <LoadingComponent />;

  return (
    <Grid stretched className="project-details-container">
      <Grid.Column width={16}>
        <ProjectDetailedHeader project={selectedProject} />
      </Grid.Column>
      <Grid.Column width={16}>
        <Segment>
          <ProjectParticipants projectId={selectedProject!.id} />
        </Segment>
      </Grid.Column>
      {/* <Grid.Column width={8}>
    <Segment style={{ height: '100%' }}>
      <ProjectDetailedInfo project={selectedProject} />
    </Segment>
  </Grid.Column> */}
      <Grid.Column width={16}>
        <Segment style={{ height: '100%' }}>
          <ProjectDetailedTicketInfo projectId={selectedProject.id} />
        </Segment>
      </Grid.Column>
    </Grid>
  );
});
