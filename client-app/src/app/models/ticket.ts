export interface Ticket {
  id: string;
  title: string;
  description: string;
  submitter: string;
  assigned: string;
  priority: string;
  severity: string;
  status: string;
  startDate: Date | null;
  endDate: Date | null;
  updated?: Date | null;
  createdAt?: Date | null;
  projectId: string;
}

export interface TicketFormValues {
  title: string;
  description: string;
  projectId: string;
}
