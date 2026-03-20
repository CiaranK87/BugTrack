export interface Notification {
  id: string;
  recipientId: string;
  type: NotificationType;
  message: string;
  isRead: boolean;
  createdAt: string;
  readAt: string | null;
  commentId: string | null;
  ticketId: string | null;
  ticketTitle: string | null;
  authorDisplayName: string | null;
  authorUsername: string | null;
}

export enum NotificationType {
  Mention = 0,
  CommentReply = 1,
  TicketAssigned = 2,
  TicketStatusChanged = 3,
}
