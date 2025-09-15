import { makeAutoObservable, runInAction } from "mobx";
import { Ticket } from "../models/ticket";
import agent from "../api/agent";
import { v4 as uuid } from "uuid";

export default class TicketStore {
  ticketRegistry = new Map<string, Ticket>();
  projectTickets = new Map<string, Ticket[]>();
  selectedTicket: Ticket | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
  }

  get ticketsByStartDate() {
    return Array.from(this.ticketRegistry.values()).sort((a, b) => a.startDate!.getTime() - b.startDate!.getTime());
  }

  loadTickets = async () => {
    this.setLoadingInitial(true);
    try {
      const tickets = await agent.Tickets.list();
      tickets.forEach((ticket) => {
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
    
    const processedTickets = tickets.map(ticket => {
      const normalize = (d: any) => d ? new Date(d + 'Z') : null;
      ticket.startDate = normalize(ticket.startDate);
      ticket.endDate = normalize(ticket.endDate);
      ticket.updated = normalize(ticket.updated);
      return ticket;
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
    
    const normalize = (d: any) => d ? new Date(d + 'Z') : null;

    ticket.startDate = normalize(ticket.startDate);
    ticket.endDate = normalize(ticket.endDate);
    ticket.updated = normalize(ticket.updated);
    this.ticketRegistry.set(ticket.id, ticket);
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
    this.loading;
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
  clear = () => {
  this.ticketRegistry.clear();
  this.selectedTicket = undefined;
}
}
