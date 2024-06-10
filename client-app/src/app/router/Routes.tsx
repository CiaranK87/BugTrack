import { Navigate, RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import ProjectForm from "../../features/projects/form/ProjectForm";
import ProjectDetails from "../../features/projects/details/ProjectDetails";
import TestError from "../../features/errors/TestError";
import NotFound from "../../features/errors/NotFound";
import ServerError from "../../features/errors/ServerError";
import LoginForm from "../../features/users/LoginForm";
import TicketDashboard from "../../features/tickets/dashboard/TicketDashboard";
import TicketDetails from "../../features/tickets/details/TicketDetails";
import TicketForm from "../../features/tickets/form/TicketForm";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    children: [
      { path: "projects", element: <ProjectDashboard /> },
      { path: "projects/:id", element: <ProjectDetails /> },
      { path: "createProject", element: <ProjectForm key="createProject" /> },
      { path: "manageProject/:id", element: <ProjectForm key="manageProject" /> },
      { path: "tickets", element: <TicketDashboard /> },
      { path: "tickets/:id", element: <TicketDetails /> },
      { path: "createTicket", element: <TicketForm key="createTicket" /> },
      { path: "manageTicket/:id", element: <TicketForm key="manageTicket" /> },
      { path: "login", element: <LoginForm /> },
      { path: "errors", element: <TestError /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
