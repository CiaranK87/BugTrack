import { useEffect, useRef } from 'react';
import { Segment, Header, Icon, Label } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../../app/stores/store';
import CommentList from './CommentList';
import CommentForm from './CommentForm';

interface Props {
  ticketId: string;
}

export default observer(function TicketComments({ ticketId }: Props) {
  const { commentStore } = useStore();
  const { connect, disconnect, connection } = commentStore;
  const isConnecting = useRef(false);

  useEffect(() => {
    if (!connection && !isConnecting.current) {
      isConnecting.current = true;
      connect(ticketId)
        .catch((error: any) => {
          console.error('Error connecting to SignalR:', error);
        })
        .finally(() => {
          isConnecting.current = false;
        });
    }

    commentStore.loadComments(ticketId);

    return () => {
      if (connection) {
        disconnect();
      }
    };
  }, [ticketId, connect, disconnect, connection, commentStore]);

  return (
    <Segment>
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '16px'
      }}>
        <Header as='h3' style={{
          fontSize: '1.2rem',
          fontWeight: 'bold',
          color: '#333',
          margin: 0
        }}>
          <Icon name='comment outline' style={{ marginRight: '8px' }} />
          Comments
        </Header>
        <Label circular style={{
          marginLeft: '10px',
          backgroundColor: '#2185d0',
          color: 'white',
          fontSize: '0.8rem',
          padding: '4px 8px'
        }}>
          {commentStore.comments.length}
        </Label>
      </div>
      <CommentForm ticketId={ticketId} />
      <CommentList ticketId={ticketId} />
    </Segment>
  );
});