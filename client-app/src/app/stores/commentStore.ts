import { makeAutoObservable, runInAction } from 'mobx';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';
import { Comment, CommentAttachment } from '../models/comment';

export default class CommentStore {
  comments: Comment[] = [];
  connection: HubConnection | null = null;
  currentTicketId: string | null = null;
  loading: boolean = false;

  constructor() {
    makeAutoObservable(this);
  }

  initConnection = () => {
    if (this.connection) {
      this.connection.stop();
      runInAction(() => {
        this.connection = null;
      });
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
      runInAction(() => {
        this.connection = null;
      });
    });

    return connection;
  };

  connect = async (ticketId: string) => {
    if (!this.connection) {
      const newConnection = this.initConnection();
      runInAction(() => {
        this.connection = newConnection;
      });
      
      await newConnection.start()
        .catch((err: any) => console.error('Error starting SignalR connection:', err));
    }

    if (this.connection) {
      await this.connection.invoke('JoinTicketGroup', ticketId)
        .catch((err: any) => console.error('Error joining ticket group:', err));
    }
      
    this.setupEventHandlers();
  };
  
  setupEventHandlers = () => {
    if (!this.connection) return;
    
    this.connection.off('ReceiveComment');
    this.connection.off('CommentUpdated');
    this.connection.off('CommentDeleted');
    this.connection.off('AttachmentAdded');
    this.connection.off('AttachmentDeleted');
    
    this.connection.on('ReceiveComment', (comment: Comment) => {
      runInAction(() => {
        if (!comment.authorUsername || !comment.authorDisplayName) {
          console.warn('Received comment without author information:', comment);
        }
        
        if (comment.parentCommentId) {
          const parentIndex = this.comments.findIndex(c => c.id === comment.parentCommentId);
          if (parentIndex !== -1) {
            if (!this.comments[parentIndex].replies) {
              this.comments[parentIndex].replies = [];
            }

            const updatedReplies = [...this.comments[parentIndex].replies, comment].sort((a, b) =>
              new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
            );
            
            const updatedParentComment = {
              ...this.comments[parentIndex],
              replies: updatedReplies
            };

            this.comments = [
              ...this.comments.slice(0, parentIndex),
              updatedParentComment,
              ...this.comments.slice(parentIndex + 1)
            ];
          } else {
            this.comments = [...this.comments, comment];
          }
        } else {
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
          if (!this.comments[commentIndex].attachments.find(a => a.id === attachment.id)) {
            const attachmentWithDownloadUrl = {
              ...attachment,
              downloadUrl: `/api/tickets/${this.currentTicketId}/comments/${commentId}/attachments/${attachment.id}/download`
            };
            this.comments[commentIndex].attachments.push(attachmentWithDownloadUrl);
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

  disconnect = () => {
    if (this.connection) {
      this.connection.stop();
      runInAction(() => {
        this.connection = null;
      });
    }
  };

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
        const commentsWithDownloadUrls = comments.map(comment => ({
          ...comment,
          attachments: comment.attachments.map(attachment => ({
            ...attachment,
            downloadUrl: attachment.downloadUrl || `/api/tickets/${comment.ticketId}/comments/${comment.id}/attachments/${attachment.id}/download`
          })),
          replies: comment.replies ? comment.replies.sort((a, b) =>
            new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
          ) : []
        }));
        
        this.comments = commentsWithDownloadUrls.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
      });
    } catch (error) {
      console.error('Error loading comments:', error);
      throw error;
    }
  };

  createComment = async (ticketId: string, content: string, attachments: File[] = [], parentCommentId?: string) => {
    this.loading = true;
    try {
      const formData = new FormData();
      formData.append('content', content);

      if (parentCommentId) {
        formData.append('parentCommentId', parentCommentId);
      }

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
        const topLevelIndex = this.comments.findIndex(c => c.id === commentId);
        if (topLevelIndex !== -1) {
          this.comments = this.comments.filter(c => c.id !== commentId);
        } else {
          const updatedComments = this.comments.map(comment => {
            if (comment.replies) {
              const updatedReplies = comment.replies.filter(reply => reply.id !== commentId);
              if (updatedReplies.length !== comment.replies.length) {
                return {
                  ...comment,
                  replies: updatedReplies
                };
              }
            }
            return comment;
          });
          
          this.comments = updatedComments;
        }
      });
    } catch (error) {
      console.error('Error deleting comment:', error);
      throw error;
    }
  };

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
          const attachmentWithDownloadUrl = {
            ...newAttachment,
            downloadUrl: newAttachment.downloadUrl || `/api/tickets/${ticketId}/comments/${commentId}/attachments/${newAttachment.id}/download`
          };
          this.comments[commentIndex].attachments.push(attachmentWithDownloadUrl);
        }
      });
    } catch (error) {
      console.error('Error adding attachment:', error);
      throw error;
    }
  };

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

  getCurrentTicketId = (): string | null => {
    return this.currentTicketId;
  };

  downloadAttachment = async (ticketId: string, commentId: string, attachmentId: string, fileName: string) => {
    try {
      const response = await fetch(`http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}/attachments/${attachmentId}/download`, {
        method: 'GET',
        credentials: 'include',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('jwt')}`
        }
      });

      if (!response.ok) {
        throw new Error(`Failed to download attachment: ${response.status} ${response.statusText}`);
      }

      const blob = await response.blob();
      
      const url = window.URL.createObjectURL(blob);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error downloading attachment:', error);
      throw error;
    }
  };

  setCurrentTicketId = (ticketId: string) => {
    this.currentTicketId = ticketId;
  };
}