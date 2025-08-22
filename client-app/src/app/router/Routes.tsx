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
import ProfilePage from "../../features/profiles/ProfilePage";
import Dashboard from "../../features/dashboard/Dashboard";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    children: [
      { path: "projects", element: <ProjectDashboard /> },
      { path: "dashboard", element: <Dashboard /> },

      
      { path: "projects/:projectId/tickets/create", element: <TicketForm key="createTicket" /> },
      { path: "projects/:projectId/tickets/:id", element: <TicketForm key="manageTicket" /> },
      { path: "projects/:id", element: <ProjectDetails /> },

      { path: "createProject", element: <ProjectForm key="createProject" /> },
      { path: "manageProject/:id", element: <ProjectForm key="manageProject" /> },
      { path: "tickets", element: <TicketDashboard /> },
      { path: "tickets/:id", element: <TicketDetails /> },
      { path: "login", element: <LoginForm /> },
      { path: "/profile/:username", element: <ProfilePage /> },
      { path: "errors", element: <TestError /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
