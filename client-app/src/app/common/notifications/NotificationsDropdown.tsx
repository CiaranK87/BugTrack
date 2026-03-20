import { observer } from "mobx-react-lite";
import { useState, useEffect, useCallback } from "react";
import { useStore } from "../../stores/store";
import { Notification, NotificationType } from "../../models/notification";
import { formatDistanceToNow, isValid } from "date-fns";
import { Dropdown, Icon, Label, Button } from "semantic-ui-react";
import { useNavigate, useLocation } from "react-router-dom";

interface Props {
  isInProfileDropdown?: boolean;
}

export default observer(function NotificationsDropdown({ isInProfileDropdown = false }: Props) {
  const { notificationStore, commonStore } = useStore();
  const { darkMode } = commonStore;
  const navigate = useNavigate();
  const location = useLocation();

  const [isOpen, setIsOpen] = useState(false);
  const [isMobileSheetOpen, setIsMobileSheetOpen] = useState(false);
  const [isMobile, setIsMobile] = useState(window.innerWidth <= 767);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth <= 767);
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  // Handle scroll to comment
  useEffect(() => {
    if (location.hash.startsWith('#comment-')) {
      const commentId = location.hash.replace('#comment-', '');
      const scrollToComment = () => {
        const commentElement = document.getElementById(`comment-${commentId}`);
        if (commentElement) {
          commentElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
          commentElement.classList.add('comment-highlight');
          setTimeout(() => {
            commentElement.classList.remove('comment-highlight');
          }, 7000);
        }
      };
      scrollToComment();
      const t1 = setTimeout(scrollToComment, 500);
      const t2 = setTimeout(scrollToComment, 1500);
      return () => { clearTimeout(t1); clearTimeout(t2); };
    }
  }, [location.hash, location.pathname]);

  const handleOpen = useCallback(async () => {
    if (isMobile && !isInProfileDropdown) {
      setIsMobileSheetOpen(true);
      document.body.style.overflow = 'hidden';
    } else {
      setIsOpen(true);
    }
    try {
      await notificationStore.loadNotifications();
    } catch (error) {
      console.error('Failed to load notifications:', error);
    }
  }, [isMobile, isInProfileDropdown, notificationStore]);

  const handleClose = useCallback(() => {
    setIsOpen(false);
    setIsMobileSheetOpen(false);
    document.body.style.overflow = '';
  }, []);

  const handleNotificationClick = (notification: Notification) => {
    if (!notification.isRead) {
      notificationStore.markAsRead(notification.id);
    }
    if (notification.ticketId) {
      const commentHash = notification.commentId ? `#comment-${notification.commentId}` : '';
      navigate(`/tickets/${notification.ticketId}${commentHash}`);
    }
    handleClose();
  };

  const safeFormatDistance = (dateString: string) => {
    try {
      const date = new Date(dateString);
      if (!isValid(date)) return "recently";
      return formatDistanceToNow(date, { addSuffix: true });
    } catch {
      return "recently";
    }
  };

  const getNotificationIcon = (type: NotificationType) => {
    switch (type) {
      case NotificationType.Mention: return { name: 'at' as const, color: 'blue' as const };
      case NotificationType.CommentReply: return { name: 'reply' as const, color: 'teal' as const };
      case NotificationType.TicketAssigned: return { name: 'user plus' as const, color: 'green' as const };
      case NotificationType.TicketStatusChanged: return { name: 'exchange' as const, color: 'orange' as const };
      default: return { name: 'bell' as const, color: 'grey' as const };
    }
  };

  const renderNotificationItem = (notification: Notification, isMobileView: boolean = false) => {
    const iconInfo = getNotificationIcon(notification.type);
    return (
      <div
        key={notification.id}
        onClick={() => handleNotificationClick(notification)}
        className="notification-item"
        style={{
          padding: isMobileView ? '14px 20px' : '12px',
          backgroundColor: !notification.isRead ? (darkMode ? '#1a3a5c' : '#f0f7ff') : 'transparent',
          cursor: 'pointer',
          borderBottom: `1px solid ${darkMode ? '#333' : '#eee'}`,
          display: 'flex',
          alignItems: 'flex-start',
          gap: '12px',
          transition: 'background-color 0.2s'
        }}
      >
        <div style={{ marginTop: '3px' }}>
          <Icon name={iconInfo.name} color={iconInfo.color} size="large" />
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ 
            fontSize: isMobileView ? '14px' : '13px', 
            fontWeight: !notification.isRead ? 'bold' : 'normal',
            color: darkMode ? '#fff' : '#333',
            marginBottom: '4px',
            lineHeight: '1.4'
          }}>
            {notification.message}
          </div>
          {notification.ticketTitle && (
            <div style={{ fontSize: '11px', color: darkMode ? '#aaa' : '#666', marginBottom: '4px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
              Ticket: {notification.ticketTitle}
            </div>
          )}
          <div style={{ fontSize: '10px', color: darkMode ? '#777' : '#999' }}>
            {safeFormatDistance(notification.createdAt)}
          </div>
        </div>
        {!notification.isRead && (
          <div style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: '#2185d0', marginTop: '6px' }} />
        )}
      </div>
    );
  };

  const renderHeader = (isMobileView: boolean = false) => (
    <div style={{
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      padding: isMobileView ? '15px 20px' : '10px 12px',
      borderBottom: `1px solid ${darkMode ? '#444' : '#eee'}`,
      backgroundColor: darkMode ? '#2d2d2d' : '#f9f9f9',
      borderTopLeftRadius: isMobileView ? '15px' : '4px',
      borderTopRightRadius: isMobileView ? '15px' : '4px'
    }}>
      <span style={{ fontWeight: 'bold', fontSize: isMobileView ? '16px' : '14px', color: darkMode ? '#fff' : '#333' }}>
        Notifications {notificationStore.unreadCount > 0 ? `(${notificationStore.unreadCount})` : ''}
      </span>
      <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
        {notificationStore.notifications.length > 0 && (
          <Button 
            basic compact size="mini" inverted={darkMode}
            style={{ margin: 0, padding: '6px 10px', fontSize: '11px' }}
            onClick={(e: any) => { e.stopPropagation(); notificationStore.markAllAsRead(); }}
            content="Mark all read"
          />
        )}
        {notificationStore.notifications.length > 0 && (
          <Icon 
            name="trash alternate" link color="red" size="large"
            style={{ margin: 0, display: 'flex', alignItems: 'center' }}
            onClick={(e: any) => { e.stopPropagation(); notificationStore.clearAllNotifications(); }}
          />
        )}
      </div>
    </div>
  );

  const renderNotificationList = (isMobileView: boolean = false) => (
    <div style={{ maxHeight: isMobileView ? 'calc(100vh - 150px)' : '400px', overflowY: 'auto', minWidth: isMobileView ? 'auto' : '320px' }}>
      {notificationStore.loading && notificationStore.notifications.length === 0 ? (
        <div style={{ padding: '20px', textAlign: 'center', color: darkMode ? '#aaa' : '#666' }}>
          <Icon name="circle notched" loading /> Loading...
        </div>
      ) : notificationStore.notifications.length === 0 ? (
        <div style={{ padding: '30px 20px', textAlign: 'center', color: darkMode ? '#aaa' : '#666' }}>
          <Icon name="bell outline" size="large" style={{ marginBottom: '10px', display: 'block', margin: '0 auto' }} />
          <p>No notifications yet</p>
        </div>
      ) : (
        notificationStore.notifications.map(n => renderNotificationItem(n, isMobileView))
      )}
    </div>
  );

  const bellTrigger = (
    <span 
      onClick={handleOpen}
      style={{ position: 'relative', display: 'inline-flex', alignItems: 'center', cursor: 'pointer', padding: '8px' }}
    >
      <Icon name="bell" size="large" style={{ margin: 0, color: '#fff' }} />
      {notificationStore.unreadCount > 0 && (
        <Label 
          circular color="red" size="mini" floating 
          style={{ top: '0', left: '100%', marginLeft: '-1.2em', marginTop: '-0.2em' }}
        >
          {notificationStore.unreadCount > 99 ? '99+' : notificationStore.unreadCount}
        </Label>
      )}
    </span>
  );

  if (isInProfileDropdown) {
    return <div style={{ width: '100%' }}>{renderHeader()}{renderNotificationList()}</div>;
  }

  return (
    <>
      {isMobile ? (
        <>
          {bellTrigger}
          {isMobileSheetOpen && (
            <>
              <div
                className="notification-overlay open"
                onClick={handleClose}
                style={{ position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', backgroundColor: 'rgba(0,0,0,0.5)', zIndex: 2500 }}
              />
              <div 
                className="notification-sheet open"
                style={{ position: 'fixed', bottom: 0, left: 0, width: '100vw', maxHeight: '85vh', backgroundColor: darkMode ? '#1e1e1e' : '#fff', zIndex: 2600, borderTopLeftRadius: '20px', borderTopRightRadius: '20px', display: 'flex', flexDirection: 'column', boxShadow: '0 -5px 25px rgba(0,0,0,0.2)' }}
              >
                <div style={{ width: '40px', height: '5px', backgroundColor: '#ddd', borderRadius: '5px', margin: '10px auto', flexShrink: 0 }} />
                {renderHeader(true)}
                {renderNotificationList(true)}
                <div style={{ padding: '15px 20px' }}><Button fluid onClick={handleClose} content="Close" /></div>
              </div>
            </>
          )}
        </>
      ) : (
        <Dropdown icon={null} trigger={bellTrigger} open={isOpen} onOpen={handleOpen} onClose={handleClose} pointing="top left">
          <Dropdown.Menu style={{ padding: 0, marginTop: '10px', backgroundColor: darkMode ? '#1e1e1e' : '#fff', border: `1px solid ${darkMode ? '#333' : '#ddd'}` }}>
            {renderHeader()}
            {renderNotificationList()}
          </Dropdown.Menu>
        </Dropdown>
      )}
    </>
  );
});
