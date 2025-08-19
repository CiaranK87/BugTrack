// features/dashboard/components/MyTasks.tsx
import { observer } from "mobx-react-lite";
import { Link } from "react-router-dom";
import { List, Segment, Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";

interface Props {
  style?: React.CSSProperties;
}

const MyTasks = ({ style }: Props) => {
  const { ticketStore, projectStore, userStore } = useStore();
  const user = userStore.user;
  const tickets = Array.from(ticketStore.ticketRegistry.values());
  const projects = Array.from(projectStore.projectRegistry.values());

  const myTasks = user
    ? tickets
        .filter(
          t =>
            t.assigned === user.username &&
            ['Open', 'In Progress', 'Pending'].includes(t.status)
        )
        .sort(
          (a, b) =>
            new Date(a.endDate || '').getTime() -
            new Date(b.endDate || '').getTime()
        )
        .slice(0, 10)
    : [];


  return (
    <Segment style={{ height: '220px', display: 'flex', flexDirection: 'column', ...style }}>
      <Header as="h3" style={{ marginBottom: 0 }}>My Tasks</Header>
      <div style={{ flexGrow: 1, overflowY: 'auto', paddingRight: '1em', fontSize: '0.95em' }}>
        {myTasks.length > 0 ? (
          <List relaxed>
            {myTasks.map(t => {
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
                      {t.endDate && ` • Due: ${new Date(t.endDate).toLocaleDateString()}`}
                    </List.Description>
                  </List.Content>
                </List.Item>
              );
            })}
          </List>
        ) : (
          <p style={{ color: 'rgba(0,0,0,0.5)', padding: '0.5em 0', fontStyle: 'italic' }}>
            No assigned tasks
          </p>
        )}
      </div>
    </Segment>
  );
};

export default observer(MyTasks);