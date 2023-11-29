import { ChangeEvent, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { Project } from "../../../app/models/project";

interface Props {
  project: Project | undefined;
  closeForm: () => void;
  createOrEdit: (project: Project) => void;
  submitting: boolean;
}

export default function ProjectForm({ project: selectedProject, closeForm, createOrEdit, submitting }: Props) {
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
    createOrEdit(project);
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
        <Button loading={submitting} floated="right" positive type="submit" content="Submit" />
        <Button onClick={closeForm} floated="right" type="button" content="Cancel" />
      </Form>
    </Segment>
  );
}
