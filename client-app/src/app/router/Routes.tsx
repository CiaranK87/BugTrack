import { RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import ProjectForm from "../../features/projects/form/ProjectForm";
import ProjectDetails from "../../features/projects/details/ProjectDetails";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    children: [
      { path: "projects", element: <ProjectDashboard /> },
      { path: "projects/:id", element: <ProjectDetails /> },
      { path: "createProject", element: <ProjectForm key="create" /> },
      { path: "manage/:id", element: <ProjectForm key="manage" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
