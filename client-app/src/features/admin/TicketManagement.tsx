import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Confirm, Modal, Icon, Label, Input } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import { Link } from "react-router-dom";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { formatDate } from "../../app/services/dateService";

export default observer(function TicketManagement() {
  const { ticketStore } = useStore();
  const { ticketsByStartDate, loadingInitial, deleteTicket } = ticketStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteTicketId, setDeleteTicketId] = useState("");
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedTicket, setSelectedTicket] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    ticketStore.loadTickets();
  }, [ticketStore]);

  const handleDelete = (ticketId: string) => {
    setDeleteTicketId(ticketId);
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    deleteTicket(deleteTicketId)
      .then(() => {
        toast.success("Ticket deleted successfully");
        ticketStore.loadTickets();
      })
      .catch(() => {
        toast.error("Failed to delete ticket");
      })
      .finally(() => {
        setConfirmOpen(false);
        setDeleteTicketId("");
      });
  };

  const handleViewDetails = (ticket: any) => {
    setSelectedTicket(ticket);
    setDetailsModalOpen(true);
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical': return 'red';
      case 'High': return 'orange';
      case 'Medium': return 'yellow';
      case 'Low': return 'green';
      default: return 'grey';
    }
  };

  const getSeverityColor = (severity: string) => {
    switch (severity) {
      case 'Critical': return 'red';
      case 'High': return 'orange';
      case 'Medium': return 'yellow';
      case 'Low': return 'green';
      default: return 'grey';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Open': return 'green';
      case 'In Progress': return 'blue';
      case 'Closed': return 'grey';
      default: return 'grey';
    }
  };

  const activeTicketsList = ticketsByStartDate.filter(ticket => !ticket.isDeleted);

  // Filter tickets based on search term
  const filteredTickets = activeTicketsList.filter(ticket =>
    ticket.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (ticket.submitter && ticket.submitter.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.assigned && ticket.assigned.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.description && ticket.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.project && ticket.project.projectTitle.toLowerCase().includes(searchTerm.toLowerCase())) ||
    ticket.priority.toLowerCase().includes(searchTerm.toLowerCase()) ||
    ticket.severity.toLowerCase().includes(searchTerm.toLowerCase()) ||
    ticket.status.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loadingInitial ? (
        <LoadingComponent content="Loading tickets..." />
      ) : (
        <Segment className="admin-ticket-management">
          <div className="admin-ticket-controls" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="ticket" content="Ticket Management" subheader="Manage all tickets" />
            <Input
              icon="search"
              placeholder="Search tickets..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="admin-ticket-search"
              style={{ width: '250px' }}
            />
          </div>

          {filteredTickets.length === 0 ? (
            <Segment placeholder>
              <Header icon>
                <Icon name="ticket" />
              </Header>
              <p>No tickets found</p>
            </Segment>
          ) : (
            <>
              <Table celled className="ticket-table-desktop">
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell>Title</Table.HeaderCell>
                    <Table.HeaderCell>Submitter</Table.HeaderCell>
                    <Table.HeaderCell>Project</Table.HeaderCell>
                    <Table.HeaderCell>Priority</Table.HeaderCell>
                    <Table.HeaderCell>Severity</Table.HeaderCell>
                    <Table.HeaderCell>Status</Table.HeaderCell>
                    <Table.HeaderCell>Created Date</Table.HeaderCell>
                    <Table.HeaderCell>Actions</Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {filteredTickets.map((ticket) => (
                    <Table.Row key={ticket.id}>
                      <Table.Cell>
                        <Header as="h4">
                          {ticket.title}
                        </Header>
                      </Table.Cell>
                      <Table.Cell>{ticket.submitter || "Unknown"}</Table.Cell>
                      <Table.Cell>
                        {ticket.project ? (
                          <Link to={`/projects/${ticket.projectId}`}>
                            {ticket.project.projectTitle}
                          </Link>
                        ) : (
                          "Unknown"
                        )}
                      </Table.Cell>
                      <Table.Cell>
                        <Label color={getPriorityColor(ticket.priority)} size="small">
                          {ticket.priority}
                        </Label>
                      </Table.Cell>
                      <Table.Cell>
                        <Label color={getSeverityColor(ticket.severity)} size="small">
                          {ticket.severity}
                        </Label>
                      </Table.Cell>
                      <Table.Cell>
                        <Label color={getStatusColor(ticket.status)} size="small">
                          {ticket.status}
                        </Label>
                      </Table.Cell>
                      <Table.Cell>
                        {ticket.createdAt ? formatDate(ticket.createdAt, 'MMM dd, yyyy') : "N/A"}
                      </Table.Cell>
                      <Table.Cell>
                        <Button.Group>
                          <Button
                            icon="eye"
                            color="blue"
                            onClick={() => handleViewDetails(ticket)}
                            title="View Details"
                          />
                          <Button
                            icon="trash"
                            color="red"
                            onClick={() => handleDelete(ticket.id)}
                            title="Delete Ticket"
                          />
                        </Button.Group>
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>

              {/* Mobile Card View */}
              <div className="ticket-cards-mobile">
                {filteredTickets.map((ticket) => (
                  <div key={ticket.id} className="ticket-mobile-card">
                    <div className="ticket-card-header">
                      <Header as="h4">
                        <Icon name="ticket" />
                        <Header.Content>
                          {ticket.title}
                          <Header.Subheader>{ticket.project?.projectTitle || "Unknown Project"}</Header.Subheader>
                        </Header.Content>
                      </Header>
                      <Label color={getStatusColor(ticket.status)} size="small" className="ticket-status-label">
                        {ticket.status}
                      </Label>
                    </div>

                    <div className="ticket-card-content">
                      <div className="ticket-detail-item">
                        <span className="detail-label">Submitter:</span>
                        <span className="detail-value">{ticket.submitter || "Unknown"}</span>
                      </div>
                      <div className="ticket-detail-item">
                        <span className="detail-label">Priority / Severity:</span>
                        <div className="detail-value">
                          <Label color={getPriorityColor(ticket.priority)} size="mini" style={{ marginRight: '5px' }}>{ticket.priority}</Label>
                          <Label color={getSeverityColor(ticket.severity)} size="mini">{ticket.severity}</Label>
                        </div>
                      </div>
                      <div className="ticket-detail-item">
                        <span className="detail-label">Created:</span>
                        <span className="detail-value">{ticket.createdAt ? formatDate(ticket.createdAt, 'MMM dd, yyyy') : "N/A"}</span>
                      </div>
                    </div>

                    <div className="ticket-card-actions">
                      <Button
                        icon="eye"
                        color="blue"
                        content="Details"
                        onClick={() => handleViewDetails(ticket)}
                        size="small"
                      />
                      <Button
                        icon="trash"
                        color="red"
                        content="Delete"
                        onClick={() => handleDelete(ticket.id)}
                        size="small"
                      />
                      <Button
                        as={Link}
                        to={`/tickets/${ticket.id}`}
                        primary
                        content="Full Page"
                        icon="external alternate"
                        size="small"
                        className="full-page-btn"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}

          <Confirm
            open={confirmOpen}
            content="Are you sure you want to delete this ticket? This will move it to the deleted tickets list."
            onCancel={() => setConfirmOpen(false)}
            onConfirm={confirmDelete}
          />

          <Modal
            open={detailsModalOpen}
            onClose={() => setDetailsModalOpen(false)}
            size="small"
            className="admin-details-modal"
            closeIcon
          >
            <Modal.Header>Ticket Details</Modal.Header>
            <Modal.Content>
              {selectedTicket && (
                <div>
                  <p><strong>Title:</strong> {selectedTicket.title}</p>
                  <p><strong>Description:</strong> {selectedTicket.description}</p>
                  <p><strong>Submitter:</strong> {selectedTicket.submitter}</p>
                  <p><strong>Assigned:</strong> {selectedTicket.assigned || "Unassigned"}</p>
                  <p><strong>Priority:</strong>
                    <Label color={getPriorityColor(selectedTicket.priority)} size="small" style={{ marginLeft: '5px' }}>
                      {selectedTicket.priority}
                    </Label>
                  </p>
                  <p><strong>Severity:</strong>
                    <Label color={getSeverityColor(selectedTicket.severity)} size="small" style={{ marginLeft: '5px' }}>
                      {selectedTicket.severity}
                    </Label>
                  </p>
                  <p><strong>Status:</strong>
                    <Label color={getStatusColor(selectedTicket.status)} size="small" style={{ marginLeft: '5px' }}>
                      {selectedTicket.status}
                    </Label>
                  </p>
                  <p><strong>Created:</strong> {selectedTicket.createdAt ? formatDate(selectedTicket.createdAt, 'MMM dd, yyyy') : "N/A"}</p>
                  {selectedTicket.updated && (
                    <p><strong>Updated:</strong> {formatDate(selectedTicket.updated, 'MMM dd, yyyy')}</p>
                  )}
                  {selectedTicket.project && (
                    <p><strong>Project:</strong>
                      <Link to={`/projects/${selectedTicket.projectId}`} style={{ marginLeft: '5px' }}>
                        {selectedTicket.project.projectTitle}
                      </Link>
                    </p>
                  )}
                </div>
              )}
            </Modal.Content>
            <Modal.Actions>
              <Button onClick={() => setDetailsModalOpen(false)}>Close</Button>
              {selectedTicket && (
                <Button as={Link} to={`/tickets/${selectedTicket.id}`} primary>
                  View Full Ticket
                </Button>
              )}
            </Modal.Actions>
          </Modal>
        </Segment>
      )}
    </div>
  );
});