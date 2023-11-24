import { useEffect, useState } from "react";
import axios from "axios";
import { Container } from "semantic-ui-react";
import { Project } from "../models/project";
import Navbar from "./Navbar";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import { v4 as uuid } from "uuid";

function App() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProject, setSelectedProject] = useState<Project | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    axios.get<Project[]>("http://localhost:5000/api/projects").then((response) => {
      setProjects(response.data);
    });
  }, []);

  function handleSelectProject(id: string) {
    setSelectedProject(projects.find((x) => x.id === id));
  }

  function handleCancelSelectedProject() {
    setSelectedProject(undefined);
  }

  function handleFormOpen(id?: string) {
    id ? handleSelectProject(id) : handleCancelSelectedProject();
    setEditMode(true);
  }

  function handleFormClose() {
    setEditMode(false);
  }

  function handleCreateOrEditProject(project: Project) {
    project.id
      ? setProjects([...projects.filter((x) => x.id !== project.id), project])
      : setProjects([...projects, { ...project, id: uuid() }]);
    setEditMode(false);
    setSelectedProject(project);
  }

  function handleDeleteProject(id: string) {
    setProjects([...projects.filter((x) => x.id !== id)]);
  }

  return (
    <>
      <Navbar openForm={handleFormOpen} />
      <Container style={{ marginTop: "7em" }}>
        <ProjectDashboard
          projects={projects}
          selectedProject={selectedProject}
          selectProject={handleSelectProject}
          cancelSelectProject={handleCancelSelectedProject}
          editMode={editMode}
          openForm={handleFormOpen}
          closeForm={handleFormClose}
          createOrEdit={handleCreateOrEditProject}
          deleteProject={handleDeleteProject}
        />
      </Container>
    </>
  );
}

export default App;
