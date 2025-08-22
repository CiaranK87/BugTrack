import { observer } from "mobx-react-lite";
import { Link } from "react-router-dom";
import { List, Segment, Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";


export default observer(function RecentTickets() {
  const { ticketStore, projectStore, userStore } = useStore();
  const user = userStore.user;
  const tickets = Array.from(ticketStore.ticketRegistry.values());
  const projects = Array.from(projectStore.projectRegistry.values());

  const recentTickets = tickets
    .filter(t => t.assigned === user?.username || t.submitter === user?.username)
    .sort((a, b) => {
      const dateA = a.updated ? new Date(a.updated).getTime() : 0;
      const dateB = b.updated ? new Date(b.updated).getTime() : 0;
      return dateB - dateA;
    })
    .slice(0, 5);

  return (
    <Segment style={{ height: '220px', display: 'flex', flexDirection: 'column' }}>
      <Header as="h3" style={{ marginBottom: 0 }}>Recent Tickets</Header>
      <div style={{ flexGrow: 1, overflowY: 'auto', paddingRight: '1em', fontSize: '0.95em' }}>
        {recentTickets.length > 0 ? (
          <List relaxed>
            {recentTickets.map(t => {
              const project = projects.find(p => p.id === t.projectId);
              return (
                <List.Item
                  key={t.id}
                  as={Link}
                  to={`/tickets/${t.id}`}
                  style={{ padding: '0.5em 0', cursor: 'pointer' }}
                >
                  <List.Content>
                    <List.Header style={{ fontWeight: 'bold', color: '#1677ff' }}>
                      {t.title}
                    </List.Header>
                    <List.Description style={{ color: 'rgba(0,0,0,0.6)' }}>
                      {project?.projectTitle} • {t.status}
                      {t.updated && ` • Updated: ${new Date(t.updated).toLocaleDateString()}`}
                    </List.Description>
                  </List.Content>
                </List.Item>
              );
            })}
          </List>
        ) : (
          <p style={{ color: 'rgba(0,0,0,0.5)', padding: '0.5em 0', fontStyle: 'italic' }}>
            No recent tickets
          </p>
        )}
      </div>
    </Segment>
  );
});