import { Grid, Segment } from "semantic-ui-react";
import { Project } from "../../../app/models/project";
import ProjectList from "./ProjectList";
import ProjectDetails from "../details/ProjectDetails";
import ProjectForm from "../form/ProjectForm";

interface Props {
  projects: Project[];
  selectedProject: Project | undefined;
  selectProject: (id: string) => void;
  cancelSelectProject: () => void;
  editMode: boolean;
  openForm: (id: string) => void;
  closeForm: () => void;
  createOrEdit: (project: Project) => void;
  deleteProject: (id: string) => void;
  submitting: boolean;
}

export default function ProjectDashboard({
  projects,
  selectedProject,
  selectProject,
  cancelSelectProject,
  editMode,
  openForm,
  closeForm,
  createOrEdit,
  deleteProject,
  submitting,
}: Props) {
  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment>
          <ProjectList projects={projects} selectProject={selectProject} deleteProject={deleteProject} submitting={submitting} />
        </Segment>
      </Grid.Column>
      <Grid.Column width={6}>
        {selectedProject && !editMode && (
          <ProjectDetails project={selectedProject} cancelSelectProject={cancelSelectProject} openForm={openForm} />
        )}
        {editMode && (
          <ProjectForm closeForm={closeForm} project={selectedProject} createOrEdit={createOrEdit} submitting={submitting} />
        )}
      </Grid.Column>
    </Grid>
  );
}
