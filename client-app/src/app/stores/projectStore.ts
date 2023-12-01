import { makeAutoObservable, runInAction } from "mobx";
import { Project } from "../models/project";
import agent from "../api/agent";
import { v4 as uuid } from "uuid";

export default class ProjectStore {
  projectRegistry = new Map<string, Project>();
  selectedProject: Project | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = true;

  constructor() {
    makeAutoObservable(this);
  }

  get projectsByStartDate() {
    return Array.from(this.projectRegistry.values()).sort((a, b) => Date.parse(a.startDate) - Date.parse(b.startDate));
  }

  loadProjects = async () => {
    try {
      const projects = await agent.Projects.list();
      projects.forEach((project) => {
        project.startDate.split("T")[0];
        this.projectRegistry.set(project.id, project);
      });
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  selectProject = (id: string) => {
    this.selectedProject = this.projectRegistry.get(id);
  };

  cancelSelectedProject = () => {
    this.selectedProject = undefined;
  };

  openForm = (id?: string) => {
    id ? this.selectProject(id) : this.cancelSelectedProject();
    this.editMode = true;
  };

  closeForm = () => {
    this.editMode = false;
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
        if (this.selectedProject?.id === id) this.cancelSelectedProject;
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
