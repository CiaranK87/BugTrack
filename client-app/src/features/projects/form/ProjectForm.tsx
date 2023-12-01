import { ChangeEvent, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";

export default observer(function ProjectForm() {
  const { projectStore } = useStore();
  const { selectedProject, closeForm, createProject, updateProject, loading } = projectStore;

  const initialState = selectedProject ?? {
    id: "",
    name: "",
    projectOwner: "",
    description: "",
    startDate: "",
    endDate: "",
  };

  const [project, setProject] = useState(initialState);

  function handleSubmit() {
    project.id ? updateProject(project) : createProject(project);
  }

  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;
    setProject({ ...project, [name]: value });
  }

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
        <Button onClick={closeForm} floated="right" type="button" content="Cancel" />
      </Form>
    </Segment>
  );
});
