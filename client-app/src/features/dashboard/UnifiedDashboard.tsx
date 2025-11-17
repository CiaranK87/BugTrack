import React, { useState, useEffect } from 'react';
import { Grid, Header, Segment, Card, Label, Icon, Button, Statistic } from 'semantic-ui-react';
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { Link } from "react-router-dom";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { safeGetDate, formatDate } from "../../app/services/dateService";

export default observer(function UnifiedDashboard() {
  const { projectStore, ticketStore, userStore } = useStore();
  const { loadProjects, projectsByStartDate } = projectStore;
  const { loadTickets, activeTickets, ticketsByStartDate } = ticketStore;
  const [loading, setLoading] = useState(true);
  const [showAllProjects, setShowAllProjects] = useState(false);
  const [showAllTickets, setShowAllTickets] = useState(false);

  useEffect(() => {
    const loadData = async () => {
      try {
        await Promise.all([loadProjects(), loadTickets()]);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, [loadProjects, loadTickets]);

  if (loading) return <LoadingComponent content="Loading Dashboard..." />;

  const activeProjects = projectsByStartDate.filter(p => !p.isDeleted && !p.isCancelled);
  const ownedProjects = activeProjects.filter(p => p.isOwner);
  const allTickets = ticketsByStartDate;

  const ticketStatusData = [
    { name: 'Open', value: allTickets.filter(t => t.status === 'Open').length, color: '#21ba45' },
    { name: 'In Progress', value: allTickets.filter(t => t.status === 'In Progress').length, color: '#2185d0' },
    { name: 'Closed', value: allTickets.filter(t => t.status === 'Closed').length, color: '#fbbd08' },
  ];

  const ticketSeverityData = [
    { name: 'Critical', value: allTickets.filter(t => t.severity === 'Critical').length },
    { name: 'High', value: allTickets.filter(t => t.severity === 'High').length },
    { name: 'Medium', value: allTickets.filter(t => t.severity === 'Medium').length },
    { name: 'Low', value: allTickets.filter(t => t.severity === 'Low').length },
  ];

  const recentTickets = allTickets
    .sort((a, b) => {
      const dateA = a.updated ? safeGetDate(a.updated) : (a.startDate ? safeGetDate(a.startDate) : new Date(0));
      const dateB = b.updated ? safeGetDate(b.updated) : (b.startDate ? safeGetDate(b.startDate) : new Date(0));
      return dateB.getTime() - dateA.getTime();
    });
  
  const recentProjects = activeProjects
    .sort((a, b) => {
      const dateA = a.startDate ? safeGetDate(a.startDate) : new Date(0);
      const dateB = b.startDate ? safeGetDate(b.startDate) : new Date(0);
      return dateB.getTime() - dateA.getTime();
    });
  
  const displayedTickets = showAllTickets ? recentTickets : recentTickets.slice(0, 3);
  const displayedProjects = showAllProjects ? recentProjects : recentProjects.slice(0, 3);


  const getSeverityColor = (severity: string) => {
    switch (severity) {
      case 'Critical': return '#ff6b6b';  
      case 'High': return '#ffa94d';
      case 'Medium': return '#ffd43b';
      case 'Low': return '#8ce99a';
      default: return '#adb5bd';
    }
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

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: '30px' }}>
        <Header as="h1" color="teal">Dashboard Overview</Header>
      </div>

      {/* Key Statistics */}
      <Grid columns={4} style={{ marginBottom: '30px' }}>
        <Grid.Column>
          <Card fluid>
            <Card.Content textAlign="center">
              <Statistic>
                <Statistic.Value>{activeProjects.length}</Statistic.Value>
                <Statistic.Label>Active Projects</Statistic.Label>
              </Statistic>
            </Card.Content>
          </Card>
        </Grid.Column>
        <Grid.Column>
          <Card fluid>
            <Card.Content textAlign="center">
              <Statistic>
                <Statistic.Value>{activeTickets.length}</Statistic.Value>
                <Statistic.Label>Active Tickets</Statistic.Label>
              </Statistic>
            </Card.Content>
          </Card>
        </Grid.Column>
        <Grid.Column>
          <Card fluid>
            <Card.Content textAlign="center">
              <Statistic>
                <Statistic.Value>{ownedProjects.length}</Statistic.Value>
                <Statistic.Label>Owned Projects</Statistic.Label>
              </Statistic>
            </Card.Content>
          </Card>
        </Grid.Column>
        <Grid.Column>
          <Card fluid>
            <Card.Content textAlign="center">
              <Statistic>
                <Statistic.Value>{allTickets.filter(t => t.submitter === userStore.user?.username).length}</Statistic.Value>
                <Statistic.Label>Submitted Tickets</Statistic.Label>
              </Statistic>
            </Card.Content>
          </Card>
        </Grid.Column>
      </Grid>

      {/* Charts */}
      <Grid columns={2} style={{ marginBottom: '30px' }}>
        <Grid.Column>
          <Segment>
            <Header as="h3">Tickets by Status</Header>
            <ResponsiveContainer width="100%" height={250}>
              <PieChart>
                <Pie
                  data={ticketStatusData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, value }) => `${name}: ${value}`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {ticketStatusData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Segment>
        </Grid.Column>
        <Grid.Column>
          <Segment>
            <Header as="h3">Tickets by Severity</Header>
            <ResponsiveContainer width="100%" height={250}>
              <BarChart data={ticketSeverityData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="value">
                  {ticketSeverityData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={getSeverityColor(entry.name)} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </Segment>
        </Grid.Column>
      </Grid>

      {/* Recent Items */}
      <Grid columns={2}>
        <Grid.Column>
          <Segment>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px' }}>
              <Header as="h3">Recent Projects</Header>
              <Button as="a" href="/projects" basic size="small">View All</Button>
            </div>
            {displayedProjects.length > 0 ? (
              displayedProjects.map(project => (
                <Card
                  fluid
                  key={project.id}
                  as={Link}
                  to={`/projects/${project.id}`}
                  style={{
                    marginBottom: '10px',
                    cursor: 'pointer',
                    transition: 'box-shadow 0.2s ease, transform 0.2s ease'
                  }}
                  onMouseEnter={(e: React.MouseEvent) => {
                    const element = e.currentTarget as HTMLElement;
                    element.style.boxShadow = '0 4px 8px rgba(0,0,0,0.15)';
                    element.style.transform = 'translateY(-2px)';
                  }}
                  onMouseLeave={(e: React.MouseEvent) => {
                    const element = e.currentTarget as HTMLElement;
                    element.style.boxShadow = '';
                    element.style.transform = '';
                  }}
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
                      {project.description.length > 80 
                        ? `${project.description.substring(0, 80)}...` 
                        : project.description}
                    </Card.Description>
                  </Card.Content>
                  <Card.Content extra>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <div>
                        {project.isOwner && (
                          <Label size="tiny" color="orange">OWNER</Label>
                        )}
                        {project.isParticipant && !project.isOwner && (
                          <Label size="tiny" color="green">PARTICIPANT</Label>
                        )}
                      </div>
                      <div>
                        <Label size="tiny" basic>
                          <Icon name="clipboard list" />
                          {project.ticketCount || 0} tickets
                        </Label>
                      </div>
                    </div>
                    <div style={{ marginTop: '5px', fontSize: '0.75em', color: '#999' }}>
                      Started: {formatDate(project.startDate, 'MMM dd, yyyy')}
                    </div>
                  </Card.Content>
                </Card>
              ))
            ) : (
              <p>No recent projects found.</p>
            )}
            {recentProjects.length > 3 && (
              <div style={{ textAlign: 'center', marginTop: '10px' }}>
                <Button
                  basic
                  size="small"
                  onClick={() => setShowAllProjects(!showAllProjects)}
                >
                  {showAllProjects ? 'Show Less' : `Show More (${recentProjects.length - 3} more)`}
                </Button>
              </div>
            )}
          </Segment>
        </Grid.Column>
        <Grid.Column>
          <Segment>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px' }}>
              <Header as="h3">Recent Tickets</Header>
              <Button as="a" href="/tickets" basic size="small">View All</Button>
            </div>
            {displayedTickets.length > 0 ? (
              displayedTickets.map(ticket => (
                <Card
                  fluid
                  key={ticket.id}
                  as={Link}
                  to={`/tickets/${ticket.id}`}
                  style={{
                    marginBottom: '10px',
                    cursor: 'pointer',
                    transition: 'box-shadow 0.2s ease, transform 0.2s ease'
                  }}
                  onMouseEnter={(e: React.MouseEvent) => {
                    const element = e.currentTarget as HTMLElement;
                    element.style.boxShadow = '0 4px 8px rgba(0,0,0,0.15)';
                    element.style.transform = 'translateY(-2px)';
                  }}
                  onMouseLeave={(e: React.MouseEvent) => {
                    const element = e.currentTarget as HTMLElement;
                    element.style.boxShadow = '';
                    element.style.transform = '';
                  }}
                >
                  <Card.Content>
                    <Card.Header>
                      {ticket.title}
                    </Card.Header>
                    <Card.Meta>
                      <span style={{ fontSize: '0.85em', color: '#666' }}>
                        #{ticket.id.slice(-6)} • Submitted by {ticket.submitter}
                      </span>
                    </Card.Meta>
                    <Card.Description>
                      {ticket.description.length > 80 
                        ? `${ticket.description.substring(0, 80)}...` 
                        : ticket.description}
                    </Card.Description>
                  </Card.Content>
                  <Card.Content extra>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <div>
                        <Label size="tiny" color={getPriorityColor(ticket.priority)}>
                          {ticket.priority}
                        </Label>
                        <Label size="tiny" basic style={{ marginLeft: '5px' }}>
                          {ticket.status}
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
                      <div style={{ marginTop: '5px', fontSize: '0.75em', color: '#999' }}>
                        Updated: {formatDate(ticket.updated, 'MMM dd, yyyy')}
                      </div>
                    )}
                  </Card.Content>
                </Card>
              ))
            ) : (
              <p>No recent tickets found.</p>
            )}
            {recentTickets.length > 3 && (
              <div style={{ textAlign: 'center', marginTop: '10px' }}>
                <Button
                  basic
                  size="small"
                  onClick={() => setShowAllTickets(!showAllTickets)}
                >
                  {showAllTickets ? 'Show Less' : `Show More (${recentTickets.length - 3} more)`}
                </Button>
              </div>
            )}
          </Segment>
        </Grid.Column>
      </Grid>
    </div>
  );
});