import { ChangeEvent, useEffect, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Project } from "../../../app/models/project";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from "uuid";

export default observer(function ProjectForm() {
  const { projectStore } = useStore();
  const { createProject, updateProject, loading, loadProject, loadingInitial } = projectStore;
  const { id } = useParams();
  const navigate = useNavigate();

  const [project, setProject] = useState<Project>({
    id: "",
    name: "",
    projectOwner: "",
    description: "",
    startDate: "",
    endDate: "",
  });

  useEffect(() => {
    if (id) loadProject(id).then((project) => setProject(project!));
  }, [id, loadProject]);

  function handleSubmit() {
    if (!project.id) {
      project.id = uuid();
      createProject(project).then(() => navigate(`/projects/${project.id}`));
    } else {
      updateProject(project).then(() => navigate(`/projects/${project.id}`));
    }
  }

  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;
    setProject({ ...project, [name]: value });
  }

  if (loadingInitial) return <LoadingComponent content="Loading Project..." />;

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete="off">
        <Form.Input placeholder="Project Name" value={project.name} name="name" onChange={handleInputChange} />
        <Form.Input placeholder="Project Owner" value={project.projectOwner} name="projectOwner" onChange={handleInputChange} />
        <Form.TextArea
          placeholder="Project Description"
          value={project.description}
          name="description"
          onChange={handleInputChange}
        />
        <Form.Input
          type="date"
          placeholder="Project start date"
          value={project.startDate}
          name="startDate"
          onChange={handleInputChange}
        />
        <Form.Input
          type="date"
          placeholder="Project end date"
          value={project.endDate}
          name="endDate"
          onChange={handleInputChange}
        />
        <Button loading={loading} floated="right" positive type="submit" content="Submit" />
        <Button as={Link} to="projects" floated="right" type="button" content="Cancel" />
      </Form>
    </Segment>
  );
});
