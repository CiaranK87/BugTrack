import { useEffect, useState } from "react";
import { Button, Header, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ProjectFormValues } from "../../../app/models/project";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Formik, Form } from "formik";
import * as Yup from "yup";
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { v4 as uuid } from "uuid";

export default observer(function ProjectForm() {
  const navigate = useNavigate();
  const { projectStore } = useStore();
  const { createProject, updateProject, loadProject, loadingInitial } = projectStore;
  const { id } = useParams();

  const [project, setProject] = useState<ProjectFormValues>(new ProjectFormValues());

  useEffect(() => {
    if (id) loadProject(id).then((project) => setProject(new ProjectFormValues(project)));
  }, [id, loadProject]);

  const validationSchema = Yup.object({
    projectTitle: Yup.string().required("The project title is required"),
    projectOwner: Yup.string().required("The project owner is required"),
    description: Yup.string().required("The project description is required"),
    startDate: Yup.string().required("The project start date is required"),
  });

  function handleFormSubmit(project: ProjectFormValues) {
    if (!project.id) {
      const newProject = {
        ...project,
        id: uuid(),
      };
      createProject(newProject).then(() => navigate(`/projects/${newProject.id}`));
    } else {
      updateProject(project).then(() => navigate(`/projects/${project.id}`));
    }
  }

  if (loadingInitial) return <LoadingComponent content="Loading Project..." />;

  return (
    <Segment clearing>
      <Header content="Project Details" sub color="teal" />
      <Formik
        validationSchema={validationSchema}
        enableReinitialize
        initialValues={project}
        onSubmit={(values) => handleFormSubmit(values)}
      >
        {({ handleSubmit, isValid, isSubmitting, dirty }) => (
          <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="projectTitle" placeholder="project Title" />
            <MyTextInput placeholder="Project Owner" name="projectOwner" />
            <MyTextArea rows={2} placeholder="Project Description" name="description" />
            <MyDateInput
              placeholderText="Project start date"
              name="startDate"
              dateFormat="MMMM d, yyyy"
            />
            <Button
              disabled={isSubmitting || !dirty || !isValid}
              loading={isSubmitting}
              floated="right"
              positive
              type="submit"
              content="Submit"
            />
            <Button as={Link} to="/projects" floated="right" type="button" content="Cancel" />
          </Form>
        )}
      </Formik>
    </Segment>
  );
});
