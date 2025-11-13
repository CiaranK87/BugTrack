import { useState, useEffect } from 'react';
import { Grid, Header, Segment, Button, Input, Dropdown, Card, Label, Icon, ButtonGroup } from 'semantic-ui-react';
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { Project } from "../../../app/models/project";
import { format } from "date-fns";

type ViewMode = 'list' | 'cards' | 'table';


const sortOptions = [
  { key: 'date-desc', text: 'Newest First', value: 'date-desc' },
  { key: 'date-asc', text: 'Oldest First', value: 'date-asc' },
  { key: 'title', text: 'Title', value: 'title' },
  { key: 'tickets', text: 'Ticket Count', value: 'tickets' },
];

export default observer(function EnhancedProjectDashboard() {
  const { projectStore, userStore } = useStore();
  const { loadProjects, projectsByStartDate } = projectStore;
  const { canCreateProjects } = userStore;
  
  const getInitialViewMode = (): ViewMode => {
    const saved = localStorage.getItem('projectDashboardViewMode');
    return saved ? saved as ViewMode : 'list';
  };
  
  const [viewMode, setViewMode] = useState<ViewMode>(getInitialViewMode);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState('date-desc');
  const [filterStatus, setFilterStatus] = useState<string[]>([]);
  const [sortDropdownOpen, setSortDropdownOpen] = useState(false);

  useEffect(() => {
    loadProjects();
  }, [loadProjects]);
  
  useEffect(() => {
    localStorage.setItem('projectDashboardViewMode', viewMode);
  }, [viewMode]);

  const getFilteredAndSortedProjects = () => {
    let filtered = projectsByStartDate.filter(p => !p.isDeleted);

    if (searchTerm) {
      filtered = filtered.filter(project =>
        project.projectTitle.toLowerCase().includes(searchTerm.toLowerCase()) ||
        project.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
        project.ownerUsername.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (filterStatus.length > 0) {
      filtered = filtered.filter(project => {
        if (filterStatus.includes('active') && !project.isCancelled) return true;
        if (filterStatus.includes('cancelled') && project.isCancelled) return true;
        if (filterStatus.includes('owner') && project.isOwner) return true;
        if (filterStatus.includes('participant') && project.isParticipant && !project.isOwner) return true;
        return false;
      });
    }

    switch (sortBy) {
      case 'date-asc':
        return filtered.sort((a, b) => new Date(a.startDate || 0).getTime() - new Date(b.startDate || 0).getTime());
      case 'date-desc':
        return filtered.sort((a, b) => new Date(b.startDate || 0).getTime() - new Date(a.startDate || 0).getTime());
      case 'title':
        return filtered.sort((a, b) => a.projectTitle.localeCompare(b.projectTitle));
      case 'tickets':
        return filtered.sort((a, b) => (b.ticketCount || 0) - (a.ticketCount || 0));
      default:
        return filtered;
    }
  };

  const renderProjectCard = (project: Project) => (
    <Card
      fluid
      key={project.id}
      style={{ cursor: 'pointer', height: '100%' }}
      onClick={() => window.location.href = `/projects/${project.id}`}
    >
      <Card.Content>
        <Card.Header>
          {project.projectTitle}
        </Card.Header>
        <Card.Meta>
          <span style={{ fontSize: '0.85em', color: '#666' }}>
            Owner: {project.owner?.displayName || project.ownerUsername}
          </span>
        </Card.Meta>
        <Card.Description>
          {project.description.length > 100
            ? `${project.description.substring(0, 100)}...`
            : project.description}
        </Card.Description>
      </Card.Content>
      <Card.Content extra style={{ marginTop: 'auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
          <div>
            {project.isOwner && (
              <Label size="tiny" color="orange">OWNER</Label>
            )}
            {project.isParticipant && !project.isOwner && (
              <Label size="tiny" color="green">PARTICIPANT</Label>
            )}
          </div>
          <div style={{ textAlign: 'right' }}>
            <Label size="tiny" basic>
              <Icon name="clipboard list" />
              {project.ticketCount || 0} tickets
            </Label>
          </div>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <Label size="tiny" color={project.isCancelled ? "red" : "green"}>
              {project.isCancelled ? "CANCELLED" : "ACTIVE"}
            </Label>
          </div>
          <div style={{ textAlign: 'right' }}>
          </div>
        </div>
        <div style={{ marginTop: '8px', fontSize: '0.75em', color: '#999' }}>
          Started: {format(new Date(project.startDate!), 'MMM dd, yyyy')}
        </div>
        {project.participants && project.participants.length > 0 && (
          <div style={{ marginTop: '5px', fontSize: '0.75em', color: '#999' }}>
            <Icon name="users" />
            {project.participants.length} participant{project.participants.length !== 1 ? 's' : ''}
          </div>
        )}
      </Card.Content>
    </Card>
  );

  const renderProjectListItem = (project: Project) => (
    <Segment
      key={project.id}
      style={{ cursor: 'pointer' }}
      onClick={() => window.location.href = `/projects/${project.id}`}
    >
      <Grid>
        <Grid.Column width={10}>
          <Header size="small">
            {project.projectTitle}
          </Header>
          <p style={{ margin: '5px 0', color: '#666' }}>
            Owner: {project.owner?.displayName || project.ownerUsername}
          </p>
          <p style={{ margin: '5px 0' }}>
            {project.description.length > 150
              ? `${project.description.substring(0, 150)}...`
              : project.description}
          </p>
        </Grid.Column>
        <Grid.Column width={6} textAlign="right">
          <div style={{ marginBottom: '10px' }}>
            <Label size="small" basic>
              <Icon name="clipboard list" />
              {project.ticketCount || 0} tickets
            </Label>
          </div>
          <div style={{ fontSize: '0.8em', color: '#999' }}>
            Started: {format(new Date(project.startDate!), 'MMM dd, yyyy')}
          </div>
          {project.participants && project.participants.length > 0 && (
            <div style={{ fontSize: '0.8em', color: '#999', marginTop: '5px' }}>
              <Icon name="users" />
              {project.participants.length} participant{project.participants.length !== 1 ? 's' : ''}
            </div>
          )}
        </Grid.Column>
      </Grid>
      <div style={{ marginTop: '10px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <Label size="small" color={project.isCancelled ? "red" : "green"}>
            {project.isCancelled ? "CANCELLED" : "ACTIVE"}
          </Label>
          {project.isOwner && (
            <Label size="small" color="orange" style={{ marginLeft: '5px' }}>OWNER</Label>
          )}
          {project.isParticipant && !project.isOwner && (
            <Label size="small" color="green" style={{ marginLeft: '5px' }}>PARTICIPANT</Label>
          )}
        </div>
        <div style={{ textAlign: 'right' }}>
        </div>
      </div>
    </Segment>
  );

  const renderProjectTableRow = (project: Project) => (
    <tr key={project.id} style={{ cursor: 'pointer' }} onClick={() => window.location.href = `/projects/${project.id}`}>
      <td>
        <Header as="h4" style={{ margin: 0 }}>
          {project.projectTitle}
        </Header>
      </td>
      <td>{project.owner?.displayName || project.ownerUsername}</td>
      <td>
        {project.isOwner && <Label size="small" color="orange">OWNER</Label>}
        {project.isParticipant && !project.isOwner && <Label size="small" color="green">PARTICIPANT</Label>}
        {!project.isOwner && !project.isParticipant && <span>-</span>}
      </td>
      <td>{project.ticketCount || 0}</td>
      <td>{project.participants?.length || 0}</td>
      <td>{format(new Date(project.startDate!), 'MMM dd, yyyy')}</td>
      <td>
        {project.isCancelled && <Label size="small" color="red">CANCELLED</Label>}
        {!project.isCancelled && <Label size="small" color="green">ACTIVE</Label>}
      </td>
    </tr>
  );

  if (projectStore.loadingInitial) return <LoadingComponent content="Loading Projects..." />;

  const filteredProjects = getFilteredAndSortedProjects();

  const statusOptions = [
    { key: 'active', text: 'Active', value: 'active' },
    { key: 'cancelled', text: 'Cancelled', value: 'cancelled' },
    { key: 'owner', text: 'Owner', value: 'owner' },
    { key: 'participant', text: 'Participant', value: 'participant' },
  ];

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ marginBottom: '20px' }}>
        <div style={{ textAlign: 'center', marginBottom: '40px' }}>
          <Header as="h2" color="teal">Project Dashboard</Header>
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '5px', flexWrap: 'wrap' }}>
          <div style={{ display: 'flex', gap: '5px', alignItems: 'center', flexWrap: 'wrap' }}>
          <Input
            icon="search"
            placeholder="Search projects..."
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
              icon="table"
              active={viewMode === 'table'}
              onClick={() => setViewMode('table')}
              title="Table View"
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
          </div>
          </div>
          {canCreateProjects && (
            <Button as="a" href="/createProject" color="teal" content="Create Project" icon="plus" />
          )}
        </div>
      </div>

      {/* Summary Statistics */}
      <div style={{ marginTop: '40px' }}>
      <Grid columns={4} style={{ marginBottom: '40px' }}>
        <Grid.Column>
          <Segment textAlign="center">
            <Header as="h3" color="teal">{filteredProjects.length}</Header>
            <p>Total Projects</p>
          </Segment>
        </Grid.Column>
        <Grid.Column>
          <Segment textAlign="center">
            <Header as="h3" color="green">
              {filteredProjects.filter(p => !p.isCancelled).length}
            </Header>
            <p>Active Projects</p>
          </Segment>
        </Grid.Column>
        <Grid.Column>
          <Segment textAlign="center">
            <Header as="h3" color="orange">
              {filteredProjects.filter(p => p.isOwner).length}
            </Header>
            <p>Owned Projects</p>
          </Segment>
        </Grid.Column>
        <Grid.Column>
          <Segment textAlign="center">
            <Header as="h3" color="blue">
              {filteredProjects.reduce((sum, p) => sum + (p.ticketCount || 0), 0)}
            </Header>
            <p>Total Tickets</p>
          </Segment>
        </Grid.Column>
      </Grid>
      </div>

      {viewMode === 'cards' && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '20px' }}>
          {filteredProjects.map((project) => (
            <div key={project.id} style={{ height: '280px' }}>
              {renderProjectCard(project)}
            </div>
          ))}
        </div>
      )}

      {viewMode === 'list' && (
        <Segment>
          <Header sub color="teal">
            Projects ({filteredProjects.length})
          </Header>
          {filteredProjects.map((project) => renderProjectListItem(project))}
        </Segment>
      )}

      {viewMode === 'table' && (
        <Segment>
          <Header sub color="teal">
            Projects ({filteredProjects.length})
          </Header>
          <table className="ui celled table" style={{ marginTop: '10px' }}>
            <thead>
              <tr>
                <th>Project Title</th>
                <th>Owner</th>
                <th>Role</th>
                <th>Tickets</th>
                <th>Participants</th>
                <th>Start Date</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredProjects.map((project) => renderProjectTableRow(project))}
            </tbody>
          </table>
        </Segment>
      )}
    </div>
  );
});