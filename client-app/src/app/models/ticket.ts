import { Project } from "./project";

export interface Ticket {
  id: string;
  title: string;
  description: string;
  submitter: string;
  assigned: string;
  priority: string;
  severity: string;
  status: string;
  closedDate: Date | null;
  startDate: Date | null;
  endDate: Date | null;
  updated?: Date | null;
  createdAt?: Date | null;
  isDeleted?: boolean;
  deletedDate?: Date | null;
  projectId: string;
  projectTitle?: string;
  project?: Project;
}

export interface TicketFormValues {
  title: string;
  description: string;
  projectId: string;
}
