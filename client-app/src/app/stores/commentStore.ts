import { makeAutoObservable, runInAction } from 'mobx';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';
import { Comment, CommentAttachment } from '../models/comment';

export default class CommentStore {
  comments: Comment[] = [];
  hubConnection: HubConnection | null = null;
  connection: HubConnection | null = null;
  currentTicketId: string | null = null;
  loading: boolean = false;

  constructor() {
    makeAutoObservable(this);
  }

  // Initialize SignalR connection
  initConnection = () => {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }

    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/comments', {
        accessTokenFactory: () => {
          const token = localStorage.getItem('jwt');
          return token || '';
        },
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(import.meta.env.DEV ? signalR.LogLevel.Warning : signalR.LogLevel.None)
      .build();

    connection.onreconnected(() => {
      console.log('SignalR reconnected');
    });

    connection.onclose(() => {
      console.log('SignalR connection closed');
      this.connection = null;
    });

    return connection;
  };

  // Connect to SignalR hub
  connect = async (ticketId: string) => {
    if (!this.connection) {
      this.connection = this.initConnection();
      
      // Start the connection
      await this.connection.start()
        .catch(err => console.error('Error starting SignalR connection:', err));
    }

    // Join the ticket group to receive real-time updates
    await this.connection.invoke('JoinTicketGroup', ticketId)
      .catch(err => console.error('Error joining ticket group:', err));
      
    // Set up event handlers
    this.setupEventHandlers();
  };
  
  // Set up SignalR event handlers
  setupEventHandlers = () => {
    if (!this.connection) return;
    
    // Clear existing handlers to avoid duplicates
    this.connection.off('ReceiveComment');
    this.connection.off('CommentUpdated');
    this.connection.off('CommentDeleted');
    this.connection.off('AttachmentAdded');
    this.connection.off('AttachmentDeleted');
    
    // Set up new handlers
    this.connection.on('ReceiveComment', (comment: Comment) => {
      runInAction(() => {
        // Check if comment already exists to avoid duplicates
        if (!this.comments.find(c => c.id === comment.id)) {
          // Add new comment to the end of the array (bottom of list)
          this.comments = [...this.comments, comment];
        }
      });
    });

    this.connection.on('CommentUpdated', (comment: Comment) => {
      runInAction(() => {
        const index = this.comments.findIndex(c => c.id === comment.id);
        if (index !== -1) {
          this.comments[index] = comment;
        }
      });
    });

    this.connection.on('CommentDeleted', (commentId: string) => {
      runInAction(() => {
        this.comments = this.comments.filter(c => c.id !== commentId);
      });
    });

    this.connection.on('AttachmentAdded', (commentId: string, attachment: CommentAttachment) => {
      runInAction(() => {
        const commentIndex = this.comments.findIndex(c => c.id === commentId);
        if (commentIndex !== -1) {
          if (!this.comments[commentIndex].attachments) {
            this.comments[commentIndex].attachments = [];
          }
          // Check if attachment already exists to avoid duplicates
          if (!this.comments[commentIndex].attachments.find(a => a.id === attachment.id)) {
            this.comments[commentIndex].attachments.push(attachment);
          }
        }
      });
    });

    this.connection.on('AttachmentDeleted', (commentId: string, attachmentId: string) => {
      runInAction(() => {
        const commentIndex = this.comments.findIndex(c => c.id === commentId);
        if (commentIndex !== -1) {
          this.comments[commentIndex].attachments = this.comments[commentIndex].attachments.filter(a => a.id !== attachmentId);
        }
      });
    });
  };

  // Disconnect from SignalR hub
  disconnect = () => {
    if (this.connection) {
      // Just stop the connection without trying to leave the group
      // The server will handle cleaning up the group when the connection closes
      this.connection.stop();
      this.connection = null;
    }
  };

  // Load comments for a ticket
  loadComments = async (ticketId: string) => {
    try {
      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments`, {
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        }
      });

      if (!response.ok) {
        throw new Error(`Failed to load comments: ${response.status} ${response.statusText}`);
      }

      const contentType = response.headers.get('content-type');
      if (!contentType || !contentType.includes('application/json')) {
        const text = await response.text();
        console.error('Non-JSON response:', text);
        throw new Error('Server returned non-JSON response');
      }

      const comments = await response.json() as Comment[];
      runInAction(() => {
        // Sort comments by creation date (oldest first, newest last)
        this.comments = comments.sort((a, b) =>
          new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
        );
      });
    } catch (error) {
      console.error('Error loading comments:', error);
      throw error;
    }
  };

  // Create a new comment
  createComment = async (ticketId: string, content: string, attachments: File[] = []) => {
    this.loading = true;
    try {
      const formData = new FormData();
      formData.append('content', content);

      // Add attachments if any
      if (attachments && attachments.length > 0) {
        for (let i = 0; i < attachments.length; i++) {
          formData.append('attachments', attachments[i]);
        }
      }

      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments`, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error('Failed to create comment');
      }

      // Don't add the comment here - it will be received through SignalR
      // This prevents duplicate comments from appearing
      runInAction(() => {
        this.loading = false;
      });
    } catch (error) {
      console.error('Error creating comment:', error);
      runInAction(() => {
        this.loading = false;
      });
      throw error;
    }
  };

  // Update an existing comment
  updateComment = async (ticketId: string, commentId: string, content: string) => {
    try {
      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}`, {
        method: 'PUT',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        },
        body: JSON.stringify({ content }),
      });

      if (!response.ok) {
        throw new Error('Failed to update comment');
      }

      const updatedComment = await response.json() as Comment;
      runInAction(() => {
        const index = this.comments.findIndex(c => c.id === commentId);
        if (index !== -1) {
          this.comments[index] = updatedComment;
        }
      });
    } catch (error) {
      console.error('Error updating comment:', error);
      throw error;
    }
  };

  // Delete a comment
  deleteComment = async (ticketId: string, commentId: string) => {
    try {
      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}`, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        },
      });

      if (!response.ok) {
        throw new Error('Failed to delete comment');
      }

      runInAction(() => {
        this.comments = this.comments.filter(c => c.id !== commentId);
      });
    } catch (error) {
      console.error('Error deleting comment:', error);
      throw error;
    }
  };

  // Add an attachment to a comment
  addAttachment = async (ticketId: string, commentId: string, file: File) => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}/attachments`, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error('Failed to add attachment');
      }

      const newAttachment = await response.json() as CommentAttachment;
      runInAction(() => {
        const commentIndex = this.comments.findIndex(c => c.id === commentId);
        if (commentIndex !== -1) {
          if (!this.comments[commentIndex].attachments) {
            this.comments[commentIndex].attachments = [];
          }
          this.comments[commentIndex].attachments.push(newAttachment);
        }
      });
    } catch (error) {
      console.error('Error adding attachment:', error);
      throw error;
    }
  };

  // Delete an attachment
  deleteAttachment = async (ticketId: string, commentId: string, attachmentId: string) => {
    try {
      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}/attachments/${attachmentId}`, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        },
      });

      if (!response.ok) {
        throw new Error('Failed to delete attachment');
      }

      runInAction(() => {
        const commentIndex = this.comments.findIndex(c => c.id === commentId);
        if (commentIndex !== -1) {
          this.comments[commentIndex].attachments = this.comments[commentIndex].attachments.filter(a => a.id !== attachmentId);
        }
      });
    } catch (error) {
      console.error('Error deleting attachment:', error);
      throw error;
    }
  };

  // Helper method to get current ticket ID (would be passed from component)
  getCurrentTicketId = (): string | null => {
    // This would be set by the component when navigating to a ticket
    return this.currentTicketId;
  };

  // Helper method to set current ticket ID
  setCurrentTicketId = (ticketId: string) => {
    this.currentTicketId = ticketId;
  };
}