import { useState, useEffect } from 'react';
import { Grid, Header, Segment, Button, Input, Dropdown, Card, Label, Icon, ButtonGroup, Confirm } from 'semantic-ui-react';
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import KanbanBoard from "../../../app/common/kanban/KanbanBoard";
import { Ticket } from "../../../app/models/ticket";
import { format } from "date-fns";
import { statusOptions } from "../../../app/common/options/statusOptions";

type ViewMode = 'list' | 'kanban' | 'cards';


const sortOptions = [
  { key: 'date-desc', text: 'Newest First', value: 'date-desc' },
  { key: 'date-asc', text: 'Oldest First', value: 'date-asc' },
  { key: 'priority', text: 'Priority', value: 'priority' },
  { key: 'severity', text: 'Severity', value: 'severity' },
  { key: 'title', text: 'Title', value: 'title' },
];

export default observer(function EnhancedTicketDashboard() {
  const { ticketStore } = useStore();
  const { loadTickets, ticketsByStartDate, updateTicket, deleteTicket, loading } = ticketStore;
  
  const getInitialViewMode = (): ViewMode => {
    const saved = localStorage.getItem('ticketDashboardViewMode');
    return saved ? saved as ViewMode : 'list';
  };
  
  const [viewMode, setViewMode] = useState<ViewMode>(getInitialViewMode);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState('date-desc');
  const [filterStatus, setFilterStatus] = useState<string[]>([]);
  const [sortDropdownOpen, setSortDropdownOpen] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteTicketId, setDeleteTicketId] = useState("");

  useEffect(() => {
    loadTickets();
  }, [loadTickets]);
  
  useEffect(() => {
    localStorage.setItem('ticketDashboardViewMode', viewMode);
  }, [viewMode]);

  const handleDragEnd = async (columns: any[]) => {
    for (const column of columns) {
      for (const ticket of column.items) {
        const originalTicket = ticketsByStartDate.find(t => t.id === ticket.id);
        if (originalTicket && originalTicket.status !== column.id) {
          try {
            await updateTicket({ ...originalTicket, status: column.id });
            await loadTickets();
          } catch (error) {
            console.error('Failed to update ticket status:', error);
            await loadTickets();
          }
          return;
        }
      }
    }
  };

  const confirmDelete = () => {
    deleteTicket(deleteTicketId)
      .then(() => {
        setConfirmOpen(false);
        setDeleteTicketId("");
      })
      .catch(() => {
        setConfirmOpen(false);
        setDeleteTicketId("");
      });
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

  const renderTicketCard = (ticket: Ticket) => (
    <Card
      fluid
      key={ticket.id}
      style={{ cursor: 'pointer', height: '100%' }}
      onClick={() => {
        window.location.href = `/tickets/${ticket.id}`;
      }}
    >
      <Card.Content>
        <Card.Header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div style={{ flex: 1, paddingRight: '10px' }}>
            {ticket.title}
          </div>
          <Label size="tiny" color={
            ticket.status === "Open" ? "green" :
            ticket.status === "In Progress" ? "blue" :
            ticket.status === "Closed" ? "grey" : "grey"
          }>
            {ticket.status}
          </Label>
        </Card.Header>
        <Card.Meta>
          <span style={{ fontSize: '0.85em', color: '#666' }}>
            #{ticket.id.slice(-6)} • Submitted by {ticket.submitter}
          </span>
        </Card.Meta>
        <Card.Description>
          {ticket.description.length > 100
            ? `${ticket.description.substring(0, 100)}...`
            : ticket.description}
        </Card.Description>
      </Card.Content>
      <Card.Content extra style={{ marginTop: 'auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
          <div>
            <Label size="tiny" color={getPriorityColor(ticket.priority)}>
              <Icon name="flag" style={{ marginRight: '3px' }} />
              P: {ticket.priority}
            </Label>
            <Label size="tiny" color={getSeverityColor(ticket.severity)} style={{ marginLeft: '5px' }}>
              <Icon name="warning sign" style={{ marginRight: '3px' }} />
              S: {ticket.severity}
            </Label>
          </div>
          {ticket.assigned && (
            <Label size="tiny" basic>
              <Icon name="user" />
              {ticket.assigned}
            </Label>
          )}
        </div>
        {ticket.updated && (
          <div style={{ marginTop: '8px', fontSize: '0.75em', color: '#999' }}>
            Updated: {format(new Date(ticket.updated + 'Z'), 'MMM dd, yyyy')}
          </div>
        )}
      </Card.Content>
    </Card>
  );

  const renderTicketListItem = (ticket: Ticket) => (
    <Segment
      key={ticket.id}
      style={{ cursor: 'pointer' }}
      onClick={() => {
        window.location.href = `/tickets/${ticket.id}`;
      }}
    >
      <Grid>
        <Grid.Column width={10}>
          <Header size="small">
            {ticket.title}
          </Header>
          <p style={{ margin: '5px 0', color: '#666' }}>
            #{ticket.id.slice(-6)} • Submitted by {ticket.submitter}
          </p>
          <p style={{ margin: '5px 0' }}>
            {ticket.description.length > 150
              ? `${ticket.description.substring(0, 150)}...`
              : ticket.description}
          </p>
        </Grid.Column>
        <Grid.Column width={6} textAlign="right">
          <div style={{ marginBottom: '10px' }}>
            <Label size="small" color={getPriorityColor(ticket.priority)}>
              <Icon name="flag" style={{ marginRight: '3px' }} />
              Priority: {ticket.priority}
            </Label>
            <Label size="small" color={getSeverityColor(ticket.severity)} style={{ marginLeft: '5px' }}>
              <Icon name="warning sign" style={{ marginRight: '3px' }} />
              Severity: {ticket.severity}
            </Label>
          </div>
          {ticket.assigned && (
            <div style={{ marginBottom: '10px' }}>
              <Label size="small" basic>
                <Icon name="user" />
                {ticket.assigned}
              </Label>
            </div>
          )}
          {ticket.updated && (
            <div style={{ fontSize: '0.8em', color: '#999' }}>
              Updated: {format(new Date(ticket.updated + 'Z'), 'MMM dd, yyyy')}
            </div>
          )}
        </Grid.Column>
      </Grid>
      <div style={{ marginTop: '10px' }}>
        <Label size="small" color={
          ticket.status === "Open" ? "green" :
          ticket.status === "In Progress" ? "blue" :
          ticket.status === "Closed" ? "grey" : "grey"
        }>
          {ticket.status}
        </Label>
      </div>
    </Segment>
  );

  const getFilteredAndSortedTickets = () => {
    let filtered = ticketsByStartDate;

    if (searchTerm) {
      filtered = filtered.filter(ticket =>
        ticket.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        ticket.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
        ticket.submitter.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (filterStatus.length > 0) {
      filtered = filtered.filter(ticket => filterStatus.includes(ticket.status));
    }

    switch (sortBy) {
      case 'date-asc':
        return filtered.sort((a, b) => new Date(a.startDate || 0).getTime() - new Date(b.startDate || 0).getTime());
      case 'date-desc':
        return filtered.sort((a, b) => new Date(b.startDate || 0).getTime() - new Date(a.startDate || 0).getTime());
      case 'priority':
        const priorityOrder = ['Critical', 'High', 'Medium', 'Low'];
        return filtered.sort((a, b) => priorityOrder.indexOf(a.priority) - priorityOrder.indexOf(b.priority));
      case 'severity':
        const severityOrder = ['Critical', 'High', 'Medium', 'Low'];
        return filtered.sort((a, b) => severityOrder.indexOf(a.severity) - severityOrder.indexOf(b.severity));
      case 'title':
        return filtered.sort((a, b) => a.title.localeCompare(b.title));
      default:
        return filtered;
    }
  };

  const getKanbanColumns = () => {
    const filteredTickets = getFilteredAndSortedTickets();

    return statusOptions.map(status => ({
      id: status.value,
      title: status.text,
      color: status.value === "Open" ? "green" : status.value === "In Progress" ? "blue" : "grey",
      items: filteredTickets.filter(ticket => ticket.status === status.value)
    }));
  };

  if (ticketStore.loadingInitial) return <LoadingComponent content="Loading Tickets..." />;

  const filteredTickets = getFilteredAndSortedTickets();

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ textAlign: 'center', marginBottom: '40px' }}>
        <Header as="h2" color="teal">Ticket Dashboard</Header>
      </div>
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '20px' }}>
        <div style={{ display: 'flex', gap: '5px', alignItems: 'center', flexWrap: 'wrap' }}>
          <Input
            icon="search"
            placeholder="Search tickets..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            style={{ width: '250px' }}
          />
          <ButtonGroup>
            <Button
              icon="th"
              active={viewMode === 'cards'}
              onClick={() => setViewMode('cards')}
              title="Card View"
            />
            <Button
              icon="list ul"
              active={viewMode === 'list'}
              onClick={() => setViewMode('list')}
              title="List View"
            />
            <Button
              icon="columns"
              active={viewMode === 'kanban'}
              onClick={() => setViewMode('kanban')}
              title="Kanban Board"
            />
          </ButtonGroup>
          <div style={{ marginLeft: '20px', display: 'flex', gap: '0px', alignItems: 'center' }}>
            <Dropdown
              open={sortDropdownOpen}
              onClick={() => setSortDropdownOpen(!sortDropdownOpen)}
              onBlur={() => setSortDropdownOpen(false)}
              options={sortOptions}
              value={sortBy}
              onChange={(_, data) => setSortBy(data.value as string)}
              style={{ minWidth: '150px' }}
              className="iconless-dropdown"
              trigger={
                <Button
                  active={true}
                  title="Sort Options"
                  icon="sort amount down"
                />
              }
            />
            {viewMode !== 'kanban' && (
              <Dropdown
                placeholder="Status Filter"
                multiple
                selection
                clearable
                options={statusOptions}
                value={filterStatus}
                onChange={(_, data) => setFilterStatus(data.value as string[])}
                style={{ minWidth: '200px' }}
              />
            )}
          </div>
        </div>
      </div>

      <div style={{ marginTop: '40px' }}>
      {viewMode === 'kanban' && (
        <KanbanBoard
          columns={getKanbanColumns()}
          onDragEnd={handleDragEnd}
          renderItem={(ticket) => renderTicketCard(ticket)}
          loading={loading}
        />
      )}

      {viewMode === 'cards' && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '20px' }}>
          {filteredTickets.map((ticket) => (
            <div key={ticket.id} style={{ height: '320px' }}>
              {renderTicketCard(ticket)}
            </div>
          ))}
        </div>
      )}

      {viewMode === 'list' && (
        <Segment>
          <Header sub color="teal">
            Active Tickets ({filteredTickets.length})
          </Header>
          {filteredTickets.map((ticket) => renderTicketListItem(ticket))}
        </Segment>
      )}
      </div>
      
      <Confirm
        open={confirmOpen}
        content="Are you sure you want to delete this ticket? This will move it to the deleted tickets list."
        onCancel={() => setConfirmOpen(false)}
        onConfirm={confirmDelete}
      />
    </div>
  );
});