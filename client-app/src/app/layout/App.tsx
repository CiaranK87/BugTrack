import { useEffect } from "react";
import { Container } from "semantic-ui-react";
import Navbar from "./Navbar";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import LoadingComponent from "./LoadingComponent";
import { useStore } from "../stores/store";
import { observer } from "mobx-react-lite";

function App() {
  const { projectStore } = useStore();

  useEffect(() => {
    projectStore.loadProjects();
  }, [projectStore]);

  if (projectStore.loadingInitial) return <LoadingComponent content="Loading App" />;

  return (
    <>
      <Navbar />
      <Container style={{ marginTop: "7em" }}>
        <ProjectDashboard />
      </Container>
    </>
  );
}

export default observer(App);
