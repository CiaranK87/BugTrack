import { Grid, Segment } from "semantic-ui-react";
import ProjectList from "./ProjectList";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import LoadingComponent from "../../../app/layout/LoadingComponent";

export default observer(function ProjectDashboard() {
  const { projectStore } = useStore();
  const { loadProjects, projectRegistry } = projectStore;

  useEffect(() => {
    if (projectRegistry.size <= 1) loadProjects();
  }, [loadProjects, projectRegistry.size]);

  if (projectStore.loadingInitial) return <LoadingComponent content="Loading App" />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment>
          <ProjectList />
        </Segment>
      </Grid.Column>
      <Grid.Column width={6}>
        <Segment textAlign="center">
          <h2>Project filters</h2>
        </Segment>
      </Grid.Column>
    </Grid>
  );
});
