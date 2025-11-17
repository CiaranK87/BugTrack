import { makeAutoObservable, runInAction } from "mobx";
import { Ticket } from "../models/ticket";
import agent from "../api/agent";
import { v4 as uuid } from "uuid";
import { priorityOptions } from "../common/options/priorityOptions";
import { severityOptions } from "../common/options/severityOptions";
import { statusOptions } from "../common/options/statusOptions";
import { normalizeTicketDates, normalizeEnumValue } from "../services/dateService";

export default class TicketStore {
  ticketRegistry = new Map<string, Ticket>();
  deletedTicketRegistry = new Map<string, Ticket>();
  projectTickets = new Map<string, Ticket[]>();
  selectedTicket: Ticket | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
  }

  get ticketsByStartDate() {
    return Array.from(this.ticketRegistry.values())
      .sort((a, b) => a.startDate!.getTime() - b.startDate!.getTime());
  }

  get activeTickets() {
    return Array.from(this.ticketRegistry.values())
      .filter(ticket => ticket.status !== "Closed" && !ticket.isDeleted)
      .sort((a, b) => a.startDate!.getTime() - b.startDate!.getTime());
  }

  get closedTickets() {
    return Array.from(this.ticketRegistry.values())
      .filter(ticket => ticket.status === "Closed" && !ticket.isDeleted)
      .sort((a, b) => (b.closedDate?.getTime() || 0) - (a.closedDate?.getTime() || 0));
  }

  loadTickets = async () => {
    this.setLoadingInitial(true);
    try {
      const tickets = await agent.Tickets.list();
      tickets.filter(ticket => !ticket.isDeleted).forEach((ticket) => {
        this.setTicket(ticket);
      });
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  };

  loadTicket = async (id: string) => {
    let ticket = this.getTicket(id);
    if (ticket) {
      this.selectedTicket = ticket;
      return ticket;
    } else {
      this.setLoadingInitial(true);
      try {
        ticket = await agent.Tickets.details(id);
        this.setTicket(ticket);
        runInAction(() => {
          this.selectedTicket = ticket;
        });
        this.setLoadingInitial(false);
        return ticket;
      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  };
  
  loadTicketsByProject = async (projectId: string) => {
  
  if (this.loadingProjectIds.has(projectId) || this.loadingInitial) return;
  
  this.loadingProjectIds.add(projectId);
  this.setLoadingInitial(true);
  
  try {
    const tickets = await agent.Tickets.listByProject(projectId);
    
    const processedTickets = tickets.filter(ticket => !ticket.isDeleted).map(ticket => {
      const allowedPriorities = priorityOptions.map(o => o.value as string);
      const allowedSeverities = severityOptions.map(o => o.value as string);
      const allowedStatuses = statusOptions.map(o => o.value as string);

      ticket.priority = normalizeEnumValue(ticket.priority, allowedPriorities);
      ticket.severity = normalizeEnumValue(ticket.severity, allowedSeverities);
      ticket.status = normalizeEnumValue(ticket.status, allowedStatuses);

      return normalizeTicketDates(ticket);
    });
    
    runInAction(() => {
      this.ticketRegistry.clear();
      processedTickets.forEach(ticket => this.ticketRegistry.set(ticket.id, ticket));
      this.projectTickets.set(projectId, processedTickets);
    });
  } catch (error) {
    console.log(error);
    runInAction(() => {
      this.projectTickets.set(projectId, []);
    });
  } finally {
    runInAction(() => {
      this.setLoadingInitial(false);
      this.loadingProjectIds.delete(projectId);
    });
  }
};


private setTicket = (ticket: Ticket) => {
  const allowedPriorities = priorityOptions.map(o => o.value as string);
  const allowedSeverities = severityOptions.map(o => o.value as string);
  const allowedStatuses = statusOptions.map(o => o.value as string);

  ticket.priority = normalizeEnumValue(ticket.priority, allowedPriorities);
  ticket.severity = normalizeEnumValue(ticket.severity, allowedSeverities);
  ticket.status = normalizeEnumValue(ticket.status, allowedStatuses);
  
  const normalizedTicket = normalizeTicketDates(ticket);
  this.ticketRegistry.set(ticket.id, normalizedTicket);
};

  private getTicket = (id: string) => {
    return this.ticketRegistry.get(id);
  };

  private loadingProjectIds = new Set<string>();
  
  getProjectTickets = (projectId: string) => {
    return this.projectTickets.get(projectId) || [];
  };
  
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };
  
  createTicket = async (ticket: Ticket) => {
    this.loading = true;
    ticket.id = uuid();

    try {
      await agent.Tickets.create(ticket);
      runInAction(() => {
        this.ticketRegistry.set(ticket.id, ticket);
        this.selectedTicket = ticket;
        this.editMode = false;
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };


  updateTicket = async (ticket: Ticket) => {
    this.loading = true;
    try {
      await agent.Tickets.update(ticket);
      const fresh = await agent.Tickets.details(ticket.id);
      runInAction(() => {
        this.setTicket(fresh);
        this.selectedTicket = fresh;
        this.editMode = false;
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  deleteTicket = async (id: string) => {
    this.loading = true;
    try {
      await agent.Tickets.delete(id);
      runInAction(() => {
        this.ticketRegistry.delete(id);
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  loadDeletedTickets = async () => {
    this.setLoadingInitial(true);
    try {
      const tickets = await agent.Tickets.listDeleted();
      runInAction(() => {
        this.deletedTicketRegistry.clear();
        tickets.forEach((ticket) => {
          this.setDeletedTicket(ticket);
        });
        this.setLoadingInitial(false);
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.setLoadingInitial(false);
      });
    }
  };

  adminDeleteTicket = async (id: string) => {
    this.loading = true;
    try {
      await agent.Tickets.adminDelete(id);
      runInAction(() => {
        this.ticketRegistry.delete(id);
        this.deletedTicketRegistry.delete(id);
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  restoreTicket = async (id: string) => {
    this.loading = true;
    try {
      await agent.Tickets.restore(id);
      runInAction(() => {
        this.deletedTicketRegistry.delete(id);
        this.loading = false;
      });
      this.loadDeletedTickets();
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  private setDeletedTicket = (ticket: Ticket) => {
    const allowedPriorities = priorityOptions.map(o => o.value as string);
    const allowedSeverities = severityOptions.map(o => o.value as string);
    const allowedStatuses = statusOptions.map(o => o.value as string);

    ticket.priority = normalizeEnumValue(ticket.priority, allowedPriorities);
    ticket.severity = normalizeEnumValue(ticket.severity, allowedSeverities);
    ticket.status = normalizeEnumValue(ticket.status, allowedStatuses);
    
    const normalizedTicket = normalizeTicketDates(ticket);
    this.deletedTicketRegistry.set(ticket.id, normalizedTicket);
  };

  get deletedTickets() {
    return Array.from(this.deletedTicketRegistry.values())
      .sort((a, b) => {
        const aTime = a.deletedDate ? new Date(a.deletedDate).getTime() : 0;
        const bTime = b.deletedDate ? new Date(b.deletedDate).getTime() : 0;
        return bTime - aTime;
      });
  }

  clear = () => {
    this.ticketRegistry.clear();
    this.deletedTicketRegistry.clear();
    this.projectTickets.clear();
    this.selectedTicket = undefined;
    this.editMode = false;
    this.loading = false;
    this.loadingInitial = false;
    this.loadingProjectIds.clear();
  }
}
