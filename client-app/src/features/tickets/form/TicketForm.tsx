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
import { severityOptions } from "../../../app/common/options/severityOptions";
import { logger } from "../../../app/utils/logger";

export default observer(function TicketForm() {
  const navigate = useNavigate();
  const { ticketStore, projectStore, userStore } = useStore();
  const { createTicket, updateTicket, loading, loadTicket, loadingInitial } = ticketStore;
  const { loadProject, projectRegistry, loadUserRoleForProject, projectRoles } = projectStore;
  const { id, projectId } = useParams<{ id: string; projectId: string }>();
  const { user, isAdmin } = userStore;

  const [ticket, setTicket] = useState<Ticket>({
    id: "",
    title: "",
    description: "",
    submitter: user?.username || "",
    assigned: "",
    priority: "",
    severity: "",
    status: "Open",
    closedDate: null,
    startDate: null,
    endDate: null,
    projectId: projectId || "",
    createdAt: null
  });

  const [projectUsersAsOptions, setProjectUsersAsOptions] = useState<{ text: string; value: string }[]>([]);
  const [loadingUserRole, setLoadingUserRole] = useState(true);

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
      setLoadingUserRole(true);

      loadUserRoleForProject(projectId).then(() => {
        setLoadingUserRole(false);
        loadProject(projectId).then(() => {
          const project = projectRegistry.get(projectId);
          const options = project?.participants?.map(p => ({
            text: p.displayName,
            value: p.username
          })) || [];
          setProjectUsersAsOptions(options);
        });
      });
    }
  }, [projectId, loadProject, projectRegistry, loadUserRoleForProject]);

  const validationSchema = Yup.object({
    title: Yup.string().required("The ticket title is required"),
    description: Yup.string().required("The ticket description is required"),
    submitter: Yup.string().required("Submitter is required"),
    assigned: Yup.string().required("Please assign to a user"),
    priority: Yup.string().required("Priority is required"),
    severity: Yup.string().required("Severity is required"),
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
        status: "Open",
        createdAt: new Date()
      };
      createTicket(newTicket).then(() => {
        projectStore.loadProjects();
        navigate(`/projects/${projectId}`);
      });

    } else {
      if (!ticket.id || typeof ticket.id !== 'string') {
        logger.error('Invalid ticket id', ticket.id);
        return;
      }

      updateTicket(ticket).then(() => {
        ticketStore.loadTicketsByProject(projectId!);
        projectStore.loadProjects();
        navigate(`/projects/${projectId}`);
      });
    }
  }

  function handleCancel() {
    navigate(`/projects/${projectId}`);
  }

  const userRole = projectRoles[projectId || ""];
  const isSubmitter = ticket.submitter === user?.username;
  const canEditTicket = isAdmin || userRole === "Owner" || userRole === "ProjectManager" || userRole === "Developer" || isSubmitter;

  if (loadingInitial || loadingUserRole) return <LoadingComponent content="Loading ticket..." />;

  if (!canEditTicket) {
    return (
      <Segment>
        <Header content="Access Denied" />
        <p>You don't have permission to create or edit tickets in this project.</p>
        <Button onClick={() => navigate(`/projects/${projectId}`)}>Back to Project</Button>
      </Segment>
    );
  }

  return (
    <Segment clearing className="admin-user-form">
      <Header content="Ticket Details" sub color="teal" />
      <div style={{ marginBottom: '20px' }}></div>
      <Formik
        validationSchema={validationSchema}
        enableReinitialize
        initialValues={ticket}
        onSubmit={(values) => handleFormSubmit(values)}
      >
        {({ handleSubmit, isValid, isSubmitting, dirty }) => (
          <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="title" placeholder="Title" label="Title" />
            <MyTextArea rows={3} placeholder="Description" name="description" label="Description" />
            <MySelectInput
              options={projectUsersAsOptions}
              placeholder="Assign to"
              name="assigned"
              label="Assigned To"
            />
            <MySelectInput options={priorityOptions} placeholder="Priority" name="priority" label="Priority" />
            <MySelectInput options={severityOptions} placeholder="Severity" name="severity" label="Severity" />
            <MyDateInput placeholderText="Start Date" name="startDate" dateFormat="MMMM d, yyyy" label="Start Date" />
            <MyDateInput placeholderText="End Date" name="endDate" dateFormat="MMMM d, yyyy" label="End Date" />

            <Button
              disabled={isSubmitting || !dirty || !isValid}
              loading={loading}
              floated="right"
              positive
              type="submit"
              content="Submit"
            />
            <Button floated="right" type="button" content="Cancel" onClick={handleCancel} />
          </Form>
        )}
      </Formik>
    </Segment>
  );
});
