import { Profile } from "./profile";

export interface IProject {
  id: string;
  projectTitle: string;
  projectOwner: string;
  description: string;
  startDate: Date | null;
  ownerUsername: string;
  isCancelled: boolean;
  isParticipant: boolean;
  isOwner: boolean;
  owner?: Profile;
  participants?: Profile[];
}

export class Project implements IProject {
  constructor(init: ProjectFormValues) {
    this.id = init.id!;
    this.projectTitle = init.projectTitle;
    this.projectOwner = init.projectOwner;
    this.description = init.description;
    this.startDate = init.startDate;
    this.ownerUsername = init.ownerUsername;
  }
  id: string;
  projectTitle: string;
  projectOwner: string;
  description: string;
  startDate: Date | null;
  ownerUsername: string;
  isCancelled: boolean = false;
  isParticipant: boolean = false;
  isOwner: boolean = false;
  owner?: Profile;
  participants?: Profile[];
  ticketCount?: number;
}

export class ProjectFormValues {
  id?: string = undefined;
  projectTitle: string = "";
  projectOwner: string = "";
  description: string = "";
  startDate: Date | null = null;
  ownerUsername: string = "";

  constructor(project?: ProjectFormValues) {
    if (project) {
      this.id = project.id;
      this.projectTitle = project.projectTitle;
      this.projectOwner = project.projectOwner;
      this.description = project.description;
      this.startDate = project.startDate;
      this.ownerUsername = project.ownerUsername;
    }
  }
}

export interface ProjectParticipantDto {
  userId: string;
  username: string;
  displayName: string;
  email: string;
  role: string;
  isOwner: boolean;
}

export interface AddParticipantDto {
  userId: string;
  role: string;
}

export interface UpdateRoleDto {
  role: string;
}
