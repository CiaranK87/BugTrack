import { Button, Header, Segment } from "semantic-ui-react";
import { useEffect, useState } from "react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useParams, useNavigate } from "react-router-dom";
import { Ticket } from "../../../app/models/ticket";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Formik, Form } from "formik";
import * as Yup from "yup";
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { priorityOptions } from "../../../app/common/options/priorityOptions";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { v4 as uuid } from "uuid";
import { statusOptions } from "../../../app/common/options/statusOptions";
import { severityOptions } from "../../../app/common/options/severityOptions";

export default observer(function TicketForm() {
  const navigate = useNavigate();
  const { ticketStore, projectStore } = useStore();
  const { createTicket, updateTicket, loading, loadTicket, loadingInitial } = ticketStore;
  const { loadProject, projectRegistry } = projectStore;
  const { id, projectId } = useParams<{ id: string; projectId: string }>();
  const { user } = useStore().userStore;

  const [ticket, setTicket] = useState<Ticket>({
    id: "",
    title: "",
    description: "",
    submitter: user?.username || "",
    assigned: "",
    priority: "",
    severity: "",
    status: "",
    startDate: null,
    endDate: null,
    projectId: projectId || "",
  });

  const [projectUsersAsOptions, setProjectUsersAsOptions] = useState<{ text: string; value: string }[]>([]);

useEffect(() => {
  if (id) {
    loadTicket(id).then((ticket) => {
      if (ticket) {
        setTicket({
          ...ticket,
          id: ticket.id,
        });
      }
    });
  }
}, [id, loadTicket]);

useEffect(() => {
  if (projectId) {
    loadProject(projectId).then(() => {
      const project = projectRegistry.get(projectId);
      const options = project?.participants?.map(p => ({
        text: p.displayName,
        value: p.username
      })) || [];
      setProjectUsersAsOptions(options);
    });
  }
}, [projectId, loadProject, projectRegistry]);

  const validationSchema = Yup.object({
    title: Yup.string().required("The ticket title is required"),
    description: Yup.string().required("The ticket description is required"),
    submitter: Yup.string().required("Submitter is required"),
    assigned: Yup.string().required("Please assign to a user"),
    priority: Yup.string().required("Priority is required"),
    severity: Yup.string().required("Severity is required"),
    status: Yup.string().required("Status is required"),
    startDate: Yup.string().required("Start date is required"),
    endDate: Yup.string().required("End date is required"),
  });

function handleFormSubmit(ticket: Ticket) {
  if (!ticket.id) {
    const newTicket = {
      ...ticket,
      id: uuid(),
      submitter: user!.username,
      projectId: projectId!,
    };
    createTicket(newTicket).then(() => {
      projectStore.loadProjects();
      navigate(`/projects/${projectId}`);
    });
      
  } else {
    if (!ticket.id || typeof ticket.id !== 'string') {
      console.error('Invalid ticket id:', ticket.id);
      return;
    }
    
    updateTicket(ticket).then(() => {
      ticketStore.loadTicketsByProject(projectId!);
      projectStore.loadProjects();
      navigate(`/projects/${projectId}`);
    });
  }
}

  if (loadingInitial) return <LoadingComponent content="Loading ticket..." />;

  return (
    <Segment clearing>
      <Header content="Ticket Details" sub color="teal" />
      <Formik
        validationSchema={validationSchema}
        enableReinitialize
        initialValues={ticket}
        onSubmit={(values) => handleFormSubmit(values)}
      >
        {({ handleSubmit, isValid, isSubmitting, dirty }) => (
          <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="title" placeholder="Title" />
            <MyTextArea rows={3} placeholder="Description" name="description" />
            <MySelectInput 
              options={projectUsersAsOptions} 
              placeholder="Assign to" 
              name="assigned" 
            />
            <MySelectInput options={priorityOptions} placeholder="Priority" name="priority" />
            <MySelectInput options={severityOptions} placeholder="Severity" name="severity" />
            <MySelectInput options={statusOptions} placeholder="Status" name="status" />
            <MyDateInput placeholderText="Start Date" name="startDate" dateFormat="MMMM d, yyyy" />
            <MyDateInput placeholderText="End Date" name="endDate" dateFormat="MMMM d, yyyy" />
            
            <Button
              disabled={isSubmitting || !dirty || !isValid}
              loading={loading}
              floated="right"
              positive
              type="submit"
              content="Submit"
            />
            <Button floated="right" type="button" content="Cancel" />
          </Form>
        )}
      </Formik>
    </Segment>
  );
});
