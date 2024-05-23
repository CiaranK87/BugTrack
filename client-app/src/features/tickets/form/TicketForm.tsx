import { Button, Form, Segment } from "semantic-ui-react";
import { ChangeEvent, useEffect, useState } from "react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useNavigate, useParams } from "react-router-dom";
import { Ticket } from "../../../app/models/ticket";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from "uuid";

export default observer(function TicketForm() {
  const { ticketStore } = useStore();
  const { createTicket, updateTicket, loading, loadTicket, loadingInitial } = ticketStore;
  const { id } = useParams();
  const navigate = useNavigate();

  const [ticket, setTicket] = useState<Ticket>({
    id: "",
    title: "",
    description: "",
    submitter: "",
    assigned: "",
    priority: "",
    severity: "",
    status: "",
    startDate: "",
    updated: "",
  });

  useEffect(() => {
    if (id) loadTicket(id).then((ticket) => setTicket(ticket!));
  }, [id, loadTicket]);

  function handleSubmit() {
    if (!ticket.id) {
      ticket.id = uuid();
      createTicket(ticket).then(() => navigate(`/tickets/${ticket.id}`));
    } else {
      updateTicket(ticket).then(() => navigate(`/tickets/${ticket.id}`));
    }
  }

  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;
    setTicket({ ...ticket, [name]: value });
  }

  if (loadingInitial) return <LoadingComponent content="Loading ticket..." />;

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete="off">
        <Form.Input placeholder="Title" value={ticket.title} name="title" onChange={handleInputChange} />
        <Form.TextArea placeholder="Description" value={ticket.description} name="description" onChange={handleInputChange} />
        <Form.Input placeholder="Submitter" value={ticket.submitter} name="submitter" onChange={handleInputChange} />
        <Form.Input placeholder="Assigned" value={ticket.assigned} name="assigned" onChange={handleInputChange} />
        <Form.Input placeholder="Priority" value={ticket.priority} name="priority" onChange={handleInputChange} />
        <Form.Input placeholder="Severity" value={ticket.severity} name="severity" onChange={handleInputChange} />
        <Form.Input placeholder="Status" value={ticket.status} name="status" onChange={handleInputChange} />
        <Form.Input type="date" placeholder="StartDate" value={ticket.startDate} name="startDate" onChange={handleInputChange} />
        <Form.Input type="date" placeholder="Updated" value={ticket.updated} name="updated" onChange={handleInputChange} />
        <Button loading={loading} floated="right" positive type="submit" content="Submit" />
        <Button floated="right" type="button" content="Cancel" />
      </Form>
    </Segment>
  );
});
