import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Confirm, Modal, Icon, Label, Input } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import { Link } from "react-router-dom";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { formatDate } from "../../app/services/dateService";

export default observer(function DeletedTicketsManagement() {
  const { ticketStore } = useStore();
  const { deletedTickets, loadingInitial, adminDeleteTicket, restoreTicket } = ticketStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteTicketId, setDeleteTicketId] = useState("");
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedTicket, setSelectedTicket] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    ticketStore.loadDeletedTickets();
  }, [ticketStore]);

  const handleDelete = (ticketId: string) => {
    setDeleteTicketId(ticketId);
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    adminDeleteTicket(deleteTicketId)
      .then(() => {
        toast.success("Ticket deleted successfully");
        ticketStore.loadDeletedTickets();
      })
      .catch(() => {
        toast.error("Failed to delete ticket");
      })
      .finally(() => {
        setConfirmOpen(false);
        setDeleteTicketId("");
      });
  };

  const handleRestore = (ticketId: string) => {
    restoreTicket(ticketId)
      .then(() => {
        toast.success("Ticket restored successfully");
      })
      .catch(() => {
        toast.error("Failed to restore ticket");
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

  const deletedTicketsList = deletedTickets;

  // Filter deleted tickets based on search term
  const filteredDeletedTickets = deletedTicketsList.filter(ticket =>
    ticket.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (ticket.submitter && ticket.submitter.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.assigned && ticket.assigned.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.description && ticket.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (ticket.project && ticket.project.projectTitle.toLowerCase().includes(searchTerm.toLowerCase())) ||
    ticket.priority.toLowerCase().includes(searchTerm.toLowerCase()) ||
    ticket.severity.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (ticket.deletedDate && formatDate(ticket.deletedDate, 'MMM dd, yyyy').toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loadingInitial ? (
        <LoadingComponent content="Loading deleted tickets..." />
      ) : (
        <Segment className="admin-deleted-tickets-management">
          <div className="admin-deleted-tickets-controls" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="trash" content="Deleted Tickets" subheader="Manage deleted tickets" />
            <Input
              icon="search"
              placeholder="Search deleted tickets..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="admin-deleted-tickets-search"
              style={{ width: '250px' }}
            />
          </div>

          {filteredDeletedTickets.length === 0 ? (
            <Segment placeholder>
              <Header icon>
                <Icon name="trash" />
              </Header>
              <p>No deleted tickets found</p>
            </Segment>
          ) : (
            <>
              <Table celled className="deleted-tickets-table-desktop">
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell>Title</Table.HeaderCell>
                    <Table.HeaderCell>Submitter</Table.HeaderCell>
                    <Table.HeaderCell>Project</Table.HeaderCell>
                    <Table.HeaderCell>Priority</Table.HeaderCell>
                    <Table.HeaderCell>Severity</Table.HeaderCell>
                    <Table.HeaderCell>Deleted Date</Table.HeaderCell>
                    <Table.HeaderCell>Actions</Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {filteredDeletedTickets.map((ticket) => (
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
                        {ticket.deletedDate ? formatDate(ticket.deletedDate, 'MMM dd, yyyy') : "N/A"}
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
                            icon="undo"
                            color="green"
                            onClick={() => handleRestore(ticket.id)}
                            title="Restore Ticket"
                          />
                          <Button
                            icon="trash"
                            color="red"
                            onClick={() => handleDelete(ticket.id)}
                            title="Permanently Delete"
                          />
                        </Button.Group>
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>

              {/* Mobile Card View */}
              <div className="deleted-ticket-cards-mobile">
                {filteredDeletedTickets.map((ticket) => (
                  <div key={ticket.id} className="ticket-mobile-card deleted-ticket-card">
                    <div className="ticket-card-header">
                      <Header as="h4">
                        <Icon name="trash" />
                        <Header.Content>
                          {ticket.title}
                          <Header.Subheader>{ticket.project?.projectTitle || "Unknown Project"}</Header.Subheader>
                        </Header.Content>
                      </Header>
                      <Label color="red" size="small">Deleted</Label>
                    </div>

                    <div className="ticket-card-content">
                      <div className="ticket-detail-item">
                        <span className="detail-label">Deleted On:</span>
                        <span className="detail-value">{ticket.deletedDate ? formatDate(ticket.deletedDate, 'MMM dd, yyyy') : "N/A"}</span>
                      </div>
                      <div className="ticket-detail-item">
                        <span className="detail-label">Priority / Severity:</span>
                        <div className="detail-value">
                          <Label color={getPriorityColor(ticket.priority)} size="mini" style={{ marginRight: '5px' }}>{ticket.priority}</Label>
                          <Label color={getSeverityColor(ticket.severity)} size="mini">{ticket.severity}</Label>
                        </div>
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
                        icon="undo"
                        color="green"
                        content="Restore"
                        onClick={() => handleRestore(ticket.id)}
                        size="small"
                      />
                      <Button
                        icon="trash"
                        color="red"
                        content="Delete"
                        onClick={() => handleDelete(ticket.id)}
                        size="small"
                        className="delete-btn"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}

          <Confirm
            open={confirmOpen}
            content="Are you sure you want to permanently delete this ticket? This action cannot be undone."
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
                  <p><strong>Status:</strong> {selectedTicket.status}</p>
                  <p><strong>Created:</strong> {selectedTicket.createdAt ? formatDate(selectedTicket.createdAt, 'MMM dd, yyyy') : "N/A"}</p>
                  <p><strong>Deleted:</strong> {selectedTicket.deletedDate ? formatDate(selectedTicket.deletedDate, 'MMM dd, yyyy') : "N/A"}</p>
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