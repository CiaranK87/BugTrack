/**
 * Service for handling date formatting and normalization
 * Provides consistent date handling across the application
 */

import { format } from "date-fns";
import { logger } from "../utils/logger";

/**
 * Safely handles date values from various sources (string, Date, null, undefined)
 * Returns a valid Date object or epoch date if invalid
 */
export const safeGetDate = (dateValue: string | Date | null | undefined): Date => {
  if (!dateValue) return new Date(0);

  try {
    if (dateValue instanceof Date) {
      if (isNaN(dateValue.getTime())) {
        logger.warn('Invalid Date object', dateValue);
        return new Date(0);
      }
      return dateValue;
    }

    // Bare ISO strings (no Z or ±offset) come from the API as DateTimeKind.Unspecified;
    // treat them as UTC to match how they were stored.
    const normalized = /^\d{4}-\d{2}-\d{2}T[\d:.]+$/.test(dateValue) ? dateValue + 'Z' : dateValue;
    const date = new Date(normalized);
    if (isNaN(date.getTime())) {
      logger.warn('Invalid date string', dateValue);
      return new Date(0);
    }
    return date;
  } catch (error) {
    logger.warn('Error handling date', { dateValue, error });
    return new Date(0);
  }
};

/**
 * Formats a date for display in the UI
 */
export const formatDate = (dateValue: string | Date | null | undefined, formatString: string = 'MMM dd, yyyy'): string => {
  const date = safeGetDate(dateValue);
  return format(date, formatString);
};

/**
 * Formats a date with time for display in the UI
 */
export const formatDateTime = (dateValue: string | Date | null | undefined, formatString: string = 'MMM dd, yyyy HH:mm'): string => {
  const date = safeGetDate(dateValue);
  return format(date, formatString);
};

/**
 * Normalizes ticket dates from API responses
 */
export const normalizeTicketDates = (ticket: any) => {
  const normalizeDate = (d: any) => d ? safeGetDate(d) : null;

  ticket.startDate = normalizeDate(ticket.startDate);
  ticket.endDate = normalizeDate(ticket.endDate);
  ticket.updatedAt = normalizeDate(ticket.updatedAt);
  ticket.createdAt = normalizeDate(ticket.createdAt);
  ticket.closedDate = normalizeDate(ticket.closedDate);
  ticket.deletedAt = normalizeDate(ticket.deletedAt);

  return ticket;
};

/**
 * Normalizes enum values (priority, severity, status)
 */
export const normalizeEnumValue = (value: string, allowedValues: string[]): string => {
  if (!value) return "";
  const trimmed = value.trim();
  const direct = allowedValues.find(a => a === trimmed);
  if (direct) return direct;
  const ci = allowedValues.find(a => a.toLowerCase() === trimmed.toLowerCase());
  return ci || "";
};