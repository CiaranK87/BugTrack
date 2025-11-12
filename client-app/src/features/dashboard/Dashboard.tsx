
import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { Container, Grid, Header, Segment, Icon } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import StatisticGroup from "./components/StatisticGroup";
import TicketStatusChart from "./components/TicketStatusChart";
import RecentTickets from "./components/RecentTickets";
import MyTasks from "./components/MyTasks";
import TicketSeverityChart from "./components/TicketSeverityChart";

export default observer(function Dashboard() {
  const { projectStore, ticketStore, userStore } = useStore();

  useEffect(() => {
    projectStore.loadProjects();
    ticketStore.loadTickets();
  }, [projectStore, ticketStore]);

  if (projectStore.loadingInitial || ticketStore.loadingInitial) {
    return (
      <Container style={{ marginTop: '7em' }}>
        <Segment>
          <Icon loading name='spinner' size='big' />
          Loading dashboard...
        </Segment>
      </Container>
    );
  }

  const projects = Array.from(projectStore.projectRegistry.values());
  const tickets = Array.from(ticketStore.ticketRegistry.values());

  const totalProjects = projects.length;
  const activeProjects = projects.filter(p => !p.isCancelled).length;
  const totalTickets = tickets.length;
  const openTickets = tickets.filter(t => t.status === 'Open').length;

  const ticketStatusData = [
    { name: 'Open', value: openTickets, color: '#21ba45' },
    { name: 'In Progress', value: tickets.filter(t => t.status === 'In Progress').length, color: '#f2c037' },
    { name: 'Closed', value: tickets.filter(t => t.status === 'Closed').length, color: '#db2828' },
  ].filter(item => item.value > 0);

  const severityData = [
    { name: 'Critical', count: tickets.filter(t => t.severity === 'Critical').length },
    { name: 'High', count: tickets.filter(t => t.severity === 'High').length },
    { name: 'Medium', count: tickets.filter(t => t.severity === 'Medium').length },
    { name: 'Low', count: tickets.filter(t => t.severity === 'Low').length },
  ].filter(item => item.count > 0);

  return (
    <Container
      style={{
        marginTop: '7em',
        height: 'calc(100vh - 7em)',
        overflow: 'hidden',
        display: 'flex',
        flexDirection: 'column'
      }}
    >
      <Header as="h1" textAlign="center" style={{ marginBottom: '1em' }}>
        Dashboard
        <Header.Subheader>
          Welcome back, {userStore.user?.displayName}
        </Header.Subheader>
      </Header>

      <StatisticGroup
        totalProjects={totalProjects}
        activeProjects={activeProjects}
        totalTickets={totalTickets}
        openTickets={openTickets}
      />

      <Grid columns={2} style={{ marginBottom: '0.5em' }}>
        <Grid.Column>
          <TicketStatusChart data={ticketStatusData} />
        </Grid.Column>
        <Grid.Column>
          <TicketSeverityChart data={severityData} />
        </Grid.Column>
      </Grid>

      <Grid columns={2} style={{ height: 240 }}>
        <Grid.Column>
          <MyTasks />
        </Grid.Column>
        <Grid.Column>
          <RecentTickets />
        </Grid.Column>
      </Grid>
    </Container>
  );
});