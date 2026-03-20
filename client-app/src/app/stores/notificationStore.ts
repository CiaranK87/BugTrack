import { makeAutoObservable, runInAction } from "mobx";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import { Notification } from "../models/notification";
import agent from "../api/agent";
import { logger } from "../utils/logger";

export default class NotificationStore {
  notifications: Notification[] = [];
  unreadCount: number = 0;
  loading: boolean = false;
  connection: HubConnection | null = null;

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

    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
    const baseUrl = apiUrl.replace('/api', '');
    const hubUrl = `${baseUrl}/hubs/notifications`;

    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          const token = localStorage.getItem('jwt');
          return token || '';
        },
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
          }
          return null;
        }
      })
      .configureLogging(import.meta.env.DEV ? signalR.LogLevel.Information : signalR.LogLevel.None)
      .build();

    connection.onclose(() => {
      runInAction(() => {
        this.connection = null;
      });
    });

    return connection;
  };

  connect = async () => {
    if (!this.connection) {
      const newConnection = this.initConnection();
      runInAction(() => {
        this.connection = newConnection;
      });
      
      await newConnection.start()
        .catch((err: any) => logger.error('Error starting Notification SignalR connection', err));
    }

    this.setupEventHandlers();
  };

  disconnect = async () => {
    if (this.connection) {
      await this.connection.stop();
      runInAction(() => {
        this.connection = null;
      });
    }
  };

  setupEventHandlers = () => {
    if (!this.connection) return;
    
    this.connection.off('ReceiveNotification');
    this.connection.off('ReceiveUnreadCount');
    
    this.connection.on('ReceiveNotification', (notification: Notification) => {
      runInAction(() => {
        this.notifications = [notification, ...this.notifications];
        if (!notification.isRead) {
          this.unreadCount += 1;
        }
      });
    });

    this.connection.on('ReceiveUnreadCount', (count: number) => {
      runInAction(() => {
        this.unreadCount = count;
      });
    });
  };

  loadNotifications = async () => {
    this.loading = true;
    try {
      const notifications = await agent.Notifications.list();
      const count = await agent.Notifications.getUnreadCount();
      runInAction(() => {
        this.notifications = notifications;
        this.unreadCount = count;
      });
    } catch (error) {
      logger.error("Failed to load notifications", error);
      throw error;
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  loadUnreadCount = async () => {
    try {
      const count = await agent.Notifications.getUnreadCount();
      runInAction(() => {
        this.unreadCount = count;
      });
    } catch (error) {
      logger.error("Failed to load unread count", error);
    }
  };

  markAsRead = async (notificationId: string) => {
    try {
      await agent.Notifications.markAsRead(notificationId);
      runInAction(() => {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification) {
          notification.isRead = true;
          notification.readAt = new Date().toISOString();
          this.unreadCount = Math.max(0, this.unreadCount - 1);
        }
      });
    } catch (error) {
      logger.error("Failed to mark notification as read", error);
      throw error;
    }
  };

  markAllAsRead = async () => {
    try {
      await agent.Notifications.markAllAsRead();
      runInAction(() => {
        this.notifications.forEach(n => {
          n.isRead = true;
          n.readAt = new Date().toISOString();
        });
        this.unreadCount = 0;
      });
    } catch (error) {
      logger.error("Failed to mark all notifications as read", error);
      throw error;
    }
  };

  deleteNotification = async (notificationId: string) => {
    try {
      await agent.Notifications.delete(notificationId);
      runInAction(() => {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification && !notification.isRead) {
          this.unreadCount = Math.max(0, this.unreadCount - 1);
        }
        this.notifications = this.notifications.filter(n => n.id !== notificationId);
      });
    } catch (error) {
      logger.error("Failed to delete notification", error);
      throw error;
    }
  };

  clearAllNotifications = async () => {
    try {
      await agent.Notifications.clearAll();
      runInAction(() => {
        this.notifications = [];
        this.unreadCount = 0;
      });
    } catch (error) {
      logger.error("Failed to clear all notifications", error);
      throw error;
    }
  };
}
