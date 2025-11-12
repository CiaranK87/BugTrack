import { Navigate, RouteObject, createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ProjectDashboard from "../../features/projects/dashboard/ProjectDashboard";
import EnhancedProjectDashboard from "../../features/projects/dashboard/EnhancedProjectDashboard";
import ProjectForm from "../../features/projects/form/ProjectForm";
import ProjectDetails from "../../features/projects/details/ProjectDetails";
import TestError from "../../features/errors/TestError";
import NotFound from "../../features/errors/NotFound";
import ServerError from "../../features/errors/ServerError";
import NetworkError from "../../features/errors/NetworkError";
import LoginForm from "../../features/users/LoginForm";
import TicketDashboard from "../../features/tickets/dashboard/TicketDashboard";
import EnhancedTicketDashboard from "../../features/tickets/dashboard/EnhancedTicketDashboard";
import TicketDetails from "../../features/tickets/details/TicketDetails";
import TicketForm from "../../features/tickets/form/TicketForm";
import ProfilePage from "../../features/profiles/ProfilePage";
import Dashboard from "../../features/dashboard/Dashboard";
import UnifiedDashboard from "../../features/dashboard/UnifiedDashboard";
import ProjectAddParticipant from "../../features/projects/details/ProjectAddParticipant";
import UserManagement from "../../features/admin/UserManagement";
import DeletedProjectsManagement from "../../features/admin/DeletedProjectsManagement";
import DeletedTicketsManagement from "../../features/admin/DeletedTicketsManagement";
import TicketManagement from "../../features/admin/TicketManagement";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <App />,
    children: [
      { path: "projects", element: <EnhancedProjectDashboard /> },
      { path: "projects/legacy", element: <ProjectDashboard /> },
      { path: "dashboard", element: <UnifiedDashboard /> },
      { path: "dashboard/legacy", element: <Dashboard /> },
      { path: "tickets", element: <EnhancedTicketDashboard /> },
      { path: "tickets/legacy", element: <TicketDashboard /> },
      { path: "admin/users", element: <UserManagement /> },
      { path: "admin/deleted-projects", element: <DeletedProjectsManagement /> },
      { path: "admin/deleted-tickets", element: <DeletedTicketsManagement /> },
      { path: "admin/tickets", element: <TicketManagement /> },

      
      { path: "projects/:projectId/tickets/create", element: <TicketForm key="createTicket" /> },
      { path: "projects/:projectId/tickets/:id", element: <TicketForm key="manageTicket" /> },
      { path: "projects/:id", element: <ProjectDetails /> },

      { path: "createProject", element: <ProjectForm key="createProject" /> },
      { path: "manageProject/:id", element: <ProjectForm key="manageProject" /> },
      { path: "tickets/:id", element: <TicketDetails /> },
      { path: "/projects/:id/participants/add", element: <ProjectAddParticipant /> },
      { path: "login", element: <LoginForm /> },
      { path: "/profile/:username", element: <ProfilePage /> },
      { path: "errors", element: <TestError /> },
      { path: "not-found", element: <NotFound /> },
      { path: "server-error", element: <ServerError /> },
      { path: "network-error", element: <NetworkError /> },
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
];

export const router = createBrowserRouter(routes);
