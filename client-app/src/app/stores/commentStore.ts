import { makeAutoObservable, runInAction } from 'mobx';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';
import { Comment, CommentAttachment } from '../models/comment';
import { logger } from '../utils/logger';
import agent from '../api/agent';
import { store } from './store';

export default class CommentStore {
  comments: Comment[] = [];
  connection: HubConnection | null = null;
  currentTicketId: string | null = null;
  loading: boolean = false;
  loadingComments: boolean = false;
  groupJoined: boolean = false;

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
      .withUrl(`${import.meta.env.VITE_API_URL?.replace('/api', '')}/hubs/comments`, {
        accessTokenFactory: () => store.commonStore.token || '',
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(import.meta.env.DEV ? signalR.LogLevel.Warning : signalR.LogLevel.None)
      .build();

    connection.onreconnected(async () => {
      logger.info('SignalR reconnected');
      runInAction(() => { this.groupJoined = false; });
      if (this.currentTicketId) {
        try {
          await this.connection?.invoke('JoinTicketGroup', this.currentTicketId);
          runInAction(() => { this.groupJoined = true; });
        } catch (err) {
          logger.error('Error re-joining ticket group after reconnect', err);
        }
      }
    });

    connection.onclose(() => {
      logger.info('SignalR connection closed');
      runInAction(() => {
        this.connection = null;
        this.groupJoined = false;
      });
    });

    return connection;
  };

  connect = async (ticketId: string) => {
    if (!this.connection) {
      const newConnection = this.initConnection();
      runInAction(() => {
        this.connection = newConnection;
        this.currentTicketId = ticketId;
      });
      
      let lastErr: unknown;
      for (let attempt = 0; attempt < 3; attempt++) {
        if (this.connection !== newConnection) return;
        if (attempt > 0) await new Promise(r => setTimeout(r, 1000));
        try {
          await newConnection.start();
          lastErr = undefined;
          break;
        } catch (err) {
          logger.error(`SignalR start failed (attempt ${attempt + 1}/3)`, err);
          lastErr = err;
        }
      }
      if (lastErr !== undefined) {
        runInAction(() => { this.connection = null; });
        throw lastErr;
      }
    }

    if (this.connection) {
      try {
        await this.connection.invoke('JoinTicketGroup', ticketId);
      } catch (err) {
        logger.error('Error joining ticket group', err);
        throw err;
      }
    }

    this.setupEventHandlers();
    runInAction(() => { this.groupJoined = true; });
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
          logger.warn('Received comment without author information', comment);
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
    }
    runInAction(() => {
      this.connection = null;
      this.groupJoined = false;
      this.comments = [];
      this.currentTicketId = null;
      this.loadingComments = false;
    });
  };

  loadComments = async (ticketId: string) => {
    runInAction(() => {
      this.currentTicketId = ticketId;
      this.loadingComments = true;
    });
    try {
      const comments = await agent.Comments.list(ticketId);
      runInAction(() => {
        if (this.currentTicketId !== ticketId) return;
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
        this.loadingComments = false;
      });
    } catch (error) {
      runInAction(() => { this.loadingComments = false; });
      logger.error('Error loading comments', error);
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

      for (const file of attachments) {
        formData.append('attachments', file);
      }

      await agent.Comments.create(ticketId, formData);
      runInAction(() => {
        this.loading = false;
      });
    } catch (error) {
      logger.error('Error creating comment', error);
      runInAction(() => {
        this.loading = false;
      });
      throw error;
    }
  };

  updateComment = async (ticketId: string, commentId: string, content: string) => {
    try {
      const updatedComment = await agent.Comments.update(ticketId, commentId, content);
      runInAction(() => {
        const index = this.comments.findIndex(c => c.id === commentId);
        if (index !== -1) {
          this.comments[index] = updatedComment;
        }
      });
    } catch (error) {
      logger.error('Error updating comment', error);
      throw error;
    }
  };

  deleteComment = async (ticketId: string, commentId: string) => {
    try {
      await agent.Comments.delete(ticketId, commentId);
      runInAction(() => {
        const topLevelIndex = this.comments.findIndex(c => c.id === commentId);
        if (topLevelIndex !== -1) {
          this.comments = this.comments.filter(c => c.id !== commentId);
        } else {
          this.comments = this.comments.map(comment => {
            if (comment.replies) {
              const updatedReplies = comment.replies.filter(reply => reply.id !== commentId);
              if (updatedReplies.length !== comment.replies.length) {
                return { ...comment, replies: updatedReplies };
              }
            }
            return comment;
          });
        }
      });
    } catch (error) {
      logger.error('Error deleting comment', error);
      throw error;
    }
  };

  deleteAttachment = async (ticketId: string, commentId: string, attachmentId: string) => {
    try {
      await agent.Comments.deleteAttachment(ticketId, commentId, attachmentId);
      runInAction(() => {
        const commentIndex = this.comments.findIndex(c => c.id === commentId);
        if (commentIndex !== -1) {
          this.comments[commentIndex].attachments = this.comments[commentIndex].attachments.filter(a => a.id !== attachmentId);
        }
      });
    } catch (error) {
      logger.error('Error deleting attachment', error);
      throw error;
    }
  };

  getCurrentTicketId = (): string | null => {
    return this.currentTicketId;
  };

  downloadAttachment = async (ticketId: string, commentId: string, attachmentId: string, fileName: string) => {
    try {
      const blob = await agent.Comments.downloadBlob(ticketId, commentId, attachmentId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      logger.error('Error downloading attachment', error);
      throw error;
    }
  };

  setCurrentTicketId = (ticketId: string) => {
    this.currentTicketId = ticketId;
  };
}