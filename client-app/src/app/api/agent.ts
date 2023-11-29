import axios, { AxiosResponse } from "axios";
import { Project } from "../models/project";

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

axios.defaults.baseURL = "http://localhost:5000/api";

axios.interceptors.response.use(async (response) => {
  try {
    await sleep(500);
    return response;
  } catch (error) {
    console.log(error);
    return await Promise.reject(error);
  }
});

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: object) => axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: object) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Projects = {
  list: () => requests.get<Project[]>("/projects"),
  details: (id: string) => requests.get<Project>(`/projects/${id}`),
  create: (project: Project) => requests.post<void>("/projects", project),
  update: (project: Project) => requests.put<void>(`/projects/${project.id}`, project),
  delete: (id: string) => requests.del<void>(`/projects/${id}`),
};

const agent = {
  Projects,
};

export default agent;
