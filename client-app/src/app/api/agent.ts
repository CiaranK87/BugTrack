import axios, { AxiosError, AxiosResponse } from "axios";
import { AddParticipantDto, Project, ProjectFormValues, ProjectParticipantDto, UpdateRoleDto } from "../models/project";
import { toast } from "react-toastify";
import { router } from "../router/Routes";
import { store } from "../stores/store";
import { User, UserFormValues, UserSearchDto, UserDto } from "../models/user";
import { Ticket } from "../models/ticket";
import { Profile } from "../models/profile";

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

axios.defaults.baseURL = "http://localhost:5000/api";

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

axios.interceptors.request.use((config) => {
  const token = store.commonStore.token;
  if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axios.interceptors.response.use(
  async (response) => {
    await sleep(500);
    return response;
  },
  (error: AxiosError) => {
    if (!error.response) {
      const currentPath = window.location.pathname;
      localStorage.setItem('lastValidPath', currentPath);
      router.navigate("/network-error", {
        state: { from: currentPath }
      });
      return Promise.reject(error);
    }

    const { data, status, config } = error.response as AxiosResponse;
    switch (status) {
      case 400:
        if (config.method === "get" && Object.prototype.hasOwnProperty.call(data.errors, "id")) {
          let fromPath = window.location.pathname;
          
          if (config.url && config.url.includes('/tickets/')) {
            fromPath = '/tickets';
          }
          else if (config.url && config.url.includes('/projects/')) {
            fromPath = '/projects';
          }
          
          router.navigate("/not-found", {
            state: { from: fromPath }
          });
        }

        if (data.errors) {
          const modalStateErrors = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key]);
            }
          }
          throw modalStateErrors.flat();
        } else {
          toast.error(data);
        }
        break;
      case 401:
        const wasLoggedIn = store.userStore.user !== null;
        store.commonStore.setToken(null);
        store.userStore.user = null;
        store.userStore.userRegistry.clear();
        store.projectStore.clear();
        store.ticketStore.clear();
        router.navigate("/");
        if (wasLoggedIn) {
          toast.error("Session expired. Please log in again.");
        }
        break;
      case 403:
        toast.error("Forbidden");
        break;
      case 404:
        let fromPath = window.location.pathname;
        
        if (config.url && config.url.includes('/tickets/')) {
          fromPath = '/tickets';
        }
        else if (config.url && config.url.includes('/projects/')) {
          fromPath = '/projects';
        }
        
        router.navigate("/not-found", {
          state: { from: fromPath }
        });
        break;
      case 500:
        store.commonStore.setServerError(data);
        router.navigate("/server-error");
        break;
    }
    return Promise.reject(error);
  }
);

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: object) => axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: object) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Projects = {
  list: () => requests.get<Project[]>("/projects"),
  details: (id: string) => requests.get<Project>(`/projects/${id}`),
  create: (project: ProjectFormValues) => requests.post<void>("/projects", project),
  update: (project: ProjectFormValues) => requests.put<void>(`/projects/${project.id}`, project),
  delete: (id: string) => requests.del<void>(`/projects/${id}`),
  adminDelete: (id: string) => requests.del<void>(`/projects/${id}/admin-delete`),
  listDeleted: () => requests.get<Project[]>("/projects/admin/deleted"),
  participate: (id: string) => requests.post<void>(`/projects/${id}/participate`, {}),
  getUserProjects: (username: string) => requests.get<Project[]>(`/profiles/${username}/projects`),
  getUserRole: (projectId: string) => requests.get<string>(`/projects/${projectId}/role`),
  listParticipants: (projectId: string) => requests.get<ProjectParticipantDto[]>(`/projects/${projectId}/participants`),
  addParticipant: (projectId: string, participant: AddParticipantDto) => requests.post<void>(`/projects/${projectId}/participants`, participant),
  updateParticipantRole: (projectId: string, userId: string, role: UpdateRoleDto) => requests.put<void>(`/projects/${projectId}/participants/${userId}`, role),
  removeParticipant: (projectId: string, userId: string) => requests.del<void>(`/projects/${projectId}/participants/${userId}`)
};

const Tickets = {
  list: () => requests.get<Ticket[]>("/tickets"),
  listByProject: (projectId: string) => requests.get<Ticket[]>(`/tickets/project/${projectId}`),
  details: (id: string) => requests.get<Ticket>(`/tickets/${id}`),
  create: (ticket: Ticket) => requests.post<void>("/tickets", ticket),
  update: (ticket: Ticket) => requests.put<void>(`/tickets/${ticket.id}`, ticket),
  delete: (id: string) => requests.del<void>(`/tickets/${id}`),
};

const Account = {
  current: () => requests.get<User>("/account"),
  login: (user: UserFormValues) => requests.post<User>("/account/login", user),
  register: (user: UserFormValues) => requests.post<User>("/account/register", user),
  adminRegister: (user: UserFormValues) => requests.post<User>("/account/admin/register", user),
};

const Profiles = {
  get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
  updateProfile: (profile: Partial<Profile>) => requests.put<void>(`/profiles`, profile),
};

const Users = {
  search: (query: string) => requests.get<UserSearchDto[]>(`/users/search?query=${query}`),
  list: () => requests.get<UserDto[]>("/users/list"),
  updateRole: (userId: string, role: string) => requests.put<UserDto>(`/users/${userId}/role`, { role }),
  update: (userId: string, user: Partial<UserDto>) => requests.put<UserDto>(`/users/${userId}`, user),
  delete: (userId: string) => requests.del<void>(`/users/${userId}`),
};

const agent = {
  Projects,
  Account,
  Tickets,
  Profiles,
  Users,
};

export default agent;
