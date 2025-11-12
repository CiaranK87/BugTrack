import { observer } from "mobx-react-lite";
import { useState } from "react";
import { Link } from "react-router-dom";
import { List, Segment, Header, Button } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";

interface Props {
  style?: React.CSSProperties;
}

const MyTasks = ({ style }: Props) => {
  const { ticketStore, projectStore, userStore } = useStore();
  const user = userStore.user;
  const [showAll, setShowAll] = useState(false);
  const tickets = Array.from(ticketStore.ticketRegistry.values());
  const projects = Array.from(projectStore.projectRegistry.values());

  const myTasks = user
    ? tickets
        .filter(
          t =>
            t.assigned === user.username &&
            ['Open', 'In Progress'].includes(t.status)
        )
        .sort(
          (a, b) =>
            new Date(a.endDate || '').getTime() -
            new Date(b.endDate || '').getTime()
        )
    : [];
  
  const displayedTasks = showAll ? myTasks : myTasks.slice(0, 3);


  return (
    <Segment style={{ height: '220px', display: 'flex', flexDirection: 'column', ...style }}>
      <Header as="h3" style={{ marginBottom: 0 }}>My Tasks</Header>
      <div style={{ flexGrow: 1, overflowY: 'auto', paddingRight: '1em', fontSize: '0.95em' }}>
        {displayedTasks.length > 0 ? (
          <List relaxed>
            {displayedTasks.map(t => {
              const project = projects.find(p => p.id === t.projectId);
              return (
                <List.Item
                  key={t.id}
                  as={Link}
                  to={`/tickets/${t.id}`}
                  style={{
                    padding: '0.5em 0',
                    cursor: 'pointer',
                    display: 'block',
                    borderRadius: '4px',
                    transition: 'background-color 0.2s ease'
                  }}
                  onMouseEnter={(e: React.MouseEvent) => (e.currentTarget as HTMLElement).style.backgroundColor = 'rgba(0,0,0,0.05)'}
                  onMouseLeave={(e: React.MouseEvent) => (e.currentTarget as HTMLElement).style.backgroundColor = 'transparent'}
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
        {myTasks.length > 3 && (
          <div style={{ textAlign: 'center', marginTop: '10px' }}>
            <Button
              basic
              size="mini"
              onClick={() => setShowAll(!showAll)}
            >
              {showAll ? 'Show Less' : `Show More (${myTasks.length - 3} more)`}
            </Button>
          </div>
        )}
      </div>
    </Segment>
  );
};

export default observer(MyTasks);