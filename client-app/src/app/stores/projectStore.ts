import { makeAutoObservable, runInAction } from "mobx";
import { Project, ProjectFormValues } from "../models/project";
import agent from "../api/agent";
import { v4 as uuid } from "uuid";
import { store } from "./store";
import { Profile } from "../models/profile";
import { router } from "../router/Routes";

export default class ProjectStore {
  projectRegistry = new Map<string, Project>();
  selectedProject: Project | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;
  userProjects: Project[] = [];
  loadingUserProjects = false;

  constructor() {
    makeAutoObservable(this);
  }

  get projectsByStartDate() {
    return Array.from(this.projectRegistry.values()).sort((a, b) => a.startDate!.getDate() - b.startDate!.getDate());
  }

  loadProjects = async () => {
    this.setLoadingInitial(true);
    try {
      this.projectRegistry.clear();
      const projects = await agent.Projects.list();
      projects.forEach((project) => {
        this.setProject(project);
      });
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  };

loadProject = async (id: string) => {
  this.setLoadingInitial(true);
  try {
    const project = await agent.Projects.details(id);
    runInAction(() => {
      this.setProject(project);
      this.selectedProject = project;
    });
    this.setLoadingInitial(false);
    return project;
  } catch (error) {
    console.log("Failed to load project", error);
    runInAction(() => {
      this.selectedProject = undefined;
    });
    this.setLoadingInitial(false);
    // Optionally: redirect
    router.navigate('/forbidden');
  }
};

  loadUserProjects = async (username: string) => {
  this.loadingUserProjects = true;
  try {
    const projects = await agent.Projects.getUserProjects(username);
    runInAction(() => {
      this.userProjects = projects;
      this.loadingUserProjects = false;
    });
  } catch (error) {
    console.log(error);
    runInAction(() => {
      this.loadingUserProjects = false;
    });
  }
}

  private getProject = (id: string) => {
    return this.projectRegistry.get(id);
  };

  private setProject = (project: Project) => {
    const user = store.userStore.user;
    if (user) {
      project.isParticipant = project.participants!.some((p) => p.username === user.username);
      project.isOwner = project.ownerUsername === user.username;
      project.owner = project.participants?.find((x) => x.username === project.ownerUsername);
    }
    project.startDate = new Date(project.startDate!);
    this.projectRegistry.set(project.id, project);
    0;
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  createProject = async (project: ProjectFormValues) => {
    const user = store.userStore.user;
    const participant = new Profile(user!);
    project.id = uuid();
    try {
      await agent.Projects.create(project);
      const newProject = new Project(project);
      newProject.ownerUsername = user!.username;
      newProject.participants = [participant];
      this.setProject(newProject);
      runInAction(() => {
        this.selectedProject = newProject;
      });
    } catch (error) {
      console.log(error);
    }
  };

  updateProject = async (project: ProjectFormValues) => {
    try {
      await agent.Projects.update(project);
      runInAction(() => {
        if (project.id) {
          const updatedProject = { ...this.getProject(project.id), ...project };
          this.projectRegistry.set(project.id, updatedProject as Project);
          this.selectedProject = project as Project;
        }
      });
    } catch (error) {
      console.log(error);
    }
  };

  deleteProject = async (id: string) => {
    this.loading = true;
    try {
      await agent.Projects.delete(id);
      runInAction(() => {
        this.projectRegistry.delete(id);
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateParticipants = async () => {
    const user = store.userStore.user;
    this.loading = true;

    try {
      await agent.Projects.participate(this.selectedProject!.id);
      runInAction(() => {
        if (this.selectedProject?.isParticipant) {
          this.selectedProject.participants = this.selectedProject.participants?.filter((p) => p.username !== user?.username);
          this.selectedProject.isParticipant = false;
        } else {
          const attendee = new Profile(user!);
          this.selectedProject?.participants?.push(attendee);
          this.selectedProject!.isParticipant = true;
        }
        this.projectRegistry.set(this.selectedProject!.id, this.selectedProject!);
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => (this.loading = false));
    }
  };

  cancelProjectToggle = async () => {
    this.loading = true;
    try {
      await agent.Projects.participate(this.selectedProject!.id);
      runInAction(() => {
        this.selectedProject!.isCancelled = !this.selectedProject?.isCancelled;
        this.projectRegistry.set(this.selectedProject!.id, this.selectedProject!);
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => (this.loading = false));
    }
  };
}
