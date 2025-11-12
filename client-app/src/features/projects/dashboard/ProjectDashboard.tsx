import { Grid, Segment } from "semantic-ui-react";
import ProjectList from "./ProjectList";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function ProjectDashboard() {
  const { projectStore } = useStore();
  const { loadProjects } = projectStore;

  useEffect(() => {
    loadProjects();
  }, [loadProjects]);

  if (projectStore.loadingInitial) return <LoadingComponent content="Loading Projects..." />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment>
          <ProjectList />
        </Segment>
      </Grid.Column>
    </Grid>
  );
});
