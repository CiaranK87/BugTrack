export enum LogLevel {
  DEBUG = 0,
  INFO = 1,
  WARN = 2,
  ERROR = 3,
  NONE = 4
}

interface LogEntry {
  timestamp: string;
  level: string;
  message: string;
  data?: any;
}

class Logger {
  private static instance: Logger;
  private logLevel: LogLevel;
  private isProduction: boolean;

  private constructor() {
    // Determine environment
    this.isProduction = import.meta.env.MODE === 'production';
    
    // Set log level based on environment
    if (this.isProduction) {
      this.logLevel = LogLevel.ERROR; // Only log errors in production
    } else {
      this.logLevel = LogLevel.DEBUG; // Log everything in development
    }
  }

  public static getInstance(): Logger {
    if (!Logger.instance) {
      Logger.instance = new Logger();
    }
    return Logger.instance;
  }

  private shouldLog(level: LogLevel): boolean {
    return level >= this.logLevel;
  }

  private formatMessage(level: string, message: string, data?: any): LogEntry {
    return {
      timestamp: new Date().toISOString(),
      level,
      message,
      data
    };
  }

  private log(level: LogLevel, levelName: string, message: string, data?: any): void {
    if (!this.shouldLog(level)) {
      return;
    }

    const logEntry = this.formatMessage(levelName, message, data);

    if (this.isProduction) {
      // In production, send to a logging service or server endpoint
      // For now, we'll just use console.error for errors
      if (level === LogLevel.ERROR) {
        console.error(JSON.stringify(logEntry));
      }
    } else {
      // In development, use console methods with styling
      const style = this.getConsoleStyle(level);
      const prefix = `%c[${logEntry.timestamp}] ${levelName}:`;
      
      switch (level) {
        case LogLevel.DEBUG:
          console.debug(prefix, style, message, data || '');
          break;
        case LogLevel.INFO:
          console.info(prefix, style, message, data || '');
          break;
        case LogLevel.WARN:
          console.warn(prefix, style, message, data || '');
          break;
        case LogLevel.ERROR:
          console.error(prefix, style, message, data || '');
          break;
      }
    }
  }

  private getConsoleStyle(level: LogLevel): string {
    switch (level) {
      case LogLevel.DEBUG:
        return 'color: #6c757d; font-weight: normal;';
      case LogLevel.INFO:
        return 'color: #007bff; font-weight: normal;';
      case LogLevel.WARN:
        return 'color: #ffc107; font-weight: bold;';
      case LogLevel.ERROR:
        return 'color: #dc3545; font-weight: bold;';
      default:
        return 'color: inherit; font-weight: normal;';
    }
  }

  public debug(message: string, data?: any): void {
    this.log(LogLevel.DEBUG, 'DEBUG', message, data);
  }

  public info(message: string, data?: any): void {
    this.log(LogLevel.INFO, 'INFO', message, data);
  }

  public warn(message: string, data?: any): void {
    this.log(LogLevel.WARN, 'WARN', message, data);
  }

  public error(message: string, data?: any): void {
    this.log(LogLevel.ERROR, 'ERROR', message, data);
  }

  // Method to change log level at runtime
  public setLogLevel(level: LogLevel): void {
    this.logLevel = level;
  }

  // Method to get current log level
  public getLogLevel(): LogLevel {
    return this.logLevel;
  }
}

// Export singleton instance
export const logger = Logger.getInstance();

// Export types for external use
export type { LogEntry };