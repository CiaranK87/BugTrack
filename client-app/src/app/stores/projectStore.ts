import { makeAutoObservable, runInAction } from "mobx";
import { Project } from "../models/project";
import agent from "../api/agent";
import { v4 as uuid } from "uuid";

export default class ProjectStore {
  projectRegistry = new Map<string, Project>();
  selectedProject: Project | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
  }

  get projectsByStartDate() {
    return Array.from(this.projectRegistry.values()).sort((a, b) => a.startDate!.getDate() - b.startDate!.getDate());
  }

  loadProjects = async () => {
    this.setLoadingInitial(true);
    try {
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
    let project = this.getProject(id);
    if (project) {
      this.selectedProject = project;
      return project;
    } else {
      this.setLoadingInitial(true);
      try {
        project = await agent.Projects.details(id);
        this.setProject(project);
        runInAction(() => (this.selectedProject = project));
        this.setLoadingInitial(false);
        return project;
      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  };

  private getProject = (id: string) => {
    return this.projectRegistry.get(id);
  };

  private setProject = (project: Project) => {
    project.startDate = new Date(project.startDate!);
    this.projectRegistry.set(project.id, project);
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  createProject = async (project: Project) => {
    this.loading = true;
    project.id = uuid();
    try {
      await agent.Projects.create(project);
      runInAction(() => {
        this.projectRegistry.set(project.id, project);
        this.selectedProject = project;
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

  updateProject = async (project: Project) => {
    this.loading = true;
    try {
      await agent.Projects.update(project);
      runInAction(() => {
        this.projectRegistry.set(project.id, project);
        this.selectedProject = project;
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
}
