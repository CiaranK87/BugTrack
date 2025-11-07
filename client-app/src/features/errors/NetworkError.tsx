import { Segment, Header, Button, Icon } from "semantic-ui-react";
import { useNavigate, useLocation } from "react-router-dom";

export default function NetworkError() {
  const navigate = useNavigate();
  const location = useLocation();

  const handleRetry = () => {
    const fromPath = location.state?.from || localStorage.getItem('lastValidPath') || '/dashboard';
    navigate(fromPath);
    setTimeout(() => window.location.reload(), 100);
  };

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <Segment placeholder>
      <Header icon textAlign="center">
        <Icon name="wifi" />
        Connection Error
      </Header>
      <p style={{ textAlign: 'center' }}>We couldn't connect to the server. Please check your internet connection and try again.</p>
      <Segment.Inline>
        <Button primary onClick={handleRetry}>
          Retry
        </Button>
        <Button onClick={handleGoHome}>
          Go to Homepage
        </Button>
      </Segment.Inline>
    </Segment>
  );
}