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
  updated: Date | null;
}
