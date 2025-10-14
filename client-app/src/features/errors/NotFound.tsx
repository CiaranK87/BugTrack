import { useNavigate, useLocation } from 'react-router-dom';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';

export default function NotFound() {
  const navigate = useNavigate();
  const location = useLocation();
  const fromPath = location.state?.from || '/projects';
  
  const getReturnPath = () => {
    if (fromPath === '/tickets' || fromPath.startsWith('/tickets/')) {
      return { path: '/tickets', label: 'Tickets' };
    }
    if (fromPath === '/projects' || fromPath.startsWith('/projects/')) {
      return { path: '/projects', label: 'Projects' };
    }
    if (location.pathname.includes('ticket')) {
      return { path: '/tickets', label: 'Tickets' };
    }
    return { path: '/projects', label: 'Projects' };
  };
  
  const { path, label } = getReturnPath();
  
  return (
    <Segment placeholder>
      <Header icon>
        <Icon name='search' />
        Oops - we've looked everywhere but couldn't find this.
      </Header>
      <Segment.Inline>
        <Button onClick={() => navigate(path)}>
          Return to {label}
        </Button>
      </Segment.Inline>
    </Segment>
  );
}





