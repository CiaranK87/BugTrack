import { useEffect, useState } from "react";
import { Container } from "semantic-ui-react";
import { Project } from "../models/project";
import Navbar from "./Navbar";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import { v4 as uuid } from "uuid";
import agent from "../api/agent";
import LoadingComponent from "./LoadingComponent";

function App() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProject, setSelectedProject] = useState<Project | undefined>(undefined);
  const [editMode, setEditMode] = useState(false);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    agent.Projects.list().then((response) => {
      let projects: Project[] = [];
      response.forEach((project) => {
        project.startDate.split("T")[0];
        projects.push(project);
      });
      setProjects(projects);
      setLoading(false);
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
    setSubmitting(true);
    if (project.id) {
      agent.Projects.update(project).then(() => {
        setProjects([...projects.filter((x) => x.id !== project.id), project]);
        setSelectedProject(project);
        setEditMode(false);
        setSubmitting(false);
      });
    } else {
      project.id = uuid();
      agent.Projects.create(project).then(() => {
        setProjects([...projects, project]);
        setSelectedProject(project);
        setEditMode(false);
        setSubmitting(false);
      });
    }
  }

  function handleDeleteProject(id: string) {
    setSubmitting(true);
    agent.Projects.delete(id).then(() => {
      setProjects([...projects.filter((x) => x.id !== id)]);
      setSubmitting(false);
    });
  }

  if (loading) return <LoadingComponent content="Loading App" />;

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
          submitting={submitting}
        />
      </Container>
    </>
  );
}

export default App;
