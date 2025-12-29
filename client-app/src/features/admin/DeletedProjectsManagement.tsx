import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Confirm, Modal, Icon, Input, Label } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import { Link } from "react-router-dom";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { format } from "date-fns";

export default observer(function DeletedProjectsManagement() {
  const { projectStore } = useStore();
  const { deletedProjects, loadingInitial, adminDeleteProject, restoreProject } = projectStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteProjectId, setDeleteProjectId] = useState("");
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedProject, setSelectedProject] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    projectStore.loadDeletedProjects();
  }, [projectStore]);

  const handleDelete = (projectId: string) => {
    setDeleteProjectId(projectId);
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    adminDeleteProject(deleteProjectId)
      .then(() => {
        toast.success("Project deleted successfully");
        projectStore.loadProjects();
        projectStore.loadDeletedProjects();
      })
      .catch(() => {
        toast.error("Failed to delete project");
      })
      .finally(() => {
        setConfirmOpen(false);
        setDeleteProjectId("");
      });
  };

  const handleRestore = (projectId: string) => {
    restoreProject(projectId)
      .then(() => {
        toast.success("Project restored successfully");
        projectStore.loadProjects();
        projectStore.loadDeletedProjects();
      })
      .catch(() => {
        toast.error("Failed to restore project");
      });
  };

  const handleViewDetails = (project: any) => {
    setSelectedProject(project);
    setDetailsModalOpen(true);
  };

  // Filter deleted projects based on search term
  const filteredDeletedProjects = deletedProjects.filter(project =>
    project.projectTitle.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (project.projectOwner && project.projectOwner.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (project.description && project.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (project.deletedDate && format(new Date(project.deletedDate), 'MMM dd, yyyy').toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loadingInitial ? (
        <LoadingComponent content="Loading deleted projects..." />
      ) : (
        <Segment className="admin-deleted-projects-management">
          <div className="admin-deleted-projects-controls" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="trash" content="Deleted Projects" subheader="Manage deleted projects" />
            <Input
              icon="search"
              placeholder="Search deleted projects..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="admin-deleted-projects-search"
              style={{ width: '250px' }}
            />
          </div>

          {filteredDeletedProjects.length === 0 ? (
            <Segment placeholder>
              <Header icon>
                <Icon name="trash" />
              </Header>
              <p>No deleted projects found</p>
            </Segment>
          ) : (
            <>
              <Table celled className="deleted-projects-table-desktop">
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell>Project Title</Table.HeaderCell>
                    <Table.HeaderCell>Owner</Table.HeaderCell>
                    <Table.HeaderCell>Start Date</Table.HeaderCell>
                    <Table.HeaderCell>Tickets</Table.HeaderCell>
                    <Table.HeaderCell>Deleted Date</Table.HeaderCell>
                    <Table.HeaderCell>Actions</Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {filteredDeletedProjects.map((project) => (
                    <Table.Row key={project.id}>
                      <Table.Cell>
                        <Header as="h4">
                          {project.projectTitle}
                        </Header>
                      </Table.Cell>
                      <Table.Cell>{project.projectOwner || "Unknown"}</Table.Cell>
                      <Table.Cell>{project.startDate ? new Date(project.startDate).toLocaleDateString() : "N/A"}</Table.Cell>
                      <Table.Cell>{project.ticketCount || 0}</Table.Cell>
                      <Table.Cell>{(project as any).deletedDate ? format(new Date((project as any).deletedDate), 'MMM dd, yyyy') : "N/A"}</Table.Cell>
                      <Table.Cell>
                        <Button.Group>
                          <Button
                            icon="eye"
                            color="blue"
                            onClick={() => handleViewDetails(project)}
                            title="View Details"
                          />
                          <Button
                            icon="undo"
                            color="green"
                            onClick={() => handleRestore(project.id)}
                            title="Restore Project"
                          />
                          <Button
                            icon="trash"
                            color="red"
                            onClick={() => handleDelete(project.id)}
                            title="Permanently Delete"
                          />
                        </Button.Group>
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>

              {/* Mobile Card View */}
              <div className="deleted-project-cards-mobile">
                {filteredDeletedProjects.map((project) => (
                  <div key={project.id} className="project-mobile-card deleted-project-card">
                    <div className="project-card-header">
                      <Header as="h4">
                        <Icon name="trash" />
                        <Header.Content>
                          {project.projectTitle}
                          <Header.Subheader>{project.projectOwner || "Unknown"}</Header.Subheader>
                        </Header.Content>
                      </Header>
                      <Label color="red" size="small">Deleted</Label>
                    </div>

                    <div className="project-card-content">
                      <div className="project-detail-item">
                        <span className="detail-label">Deleted On:</span>
                        <span className="detail-value">{(project as any).deletedDate ? format(new Date((project as any).deletedDate), 'MMM dd, yyyy') : "N/A"}</span>
                      </div>
                      <div className="project-detail-item">
                        <span className="detail-label">Tickets:</span>
                        <span className="detail-value">{project.ticketCount || 0}</span>
                      </div>
                    </div>

                    <div className="project-card-actions">
                      <Button
                        icon="eye"
                        color="blue"
                        content="Details"
                        onClick={() => handleViewDetails(project)}
                        size="small"
                      />
                      <Button
                        icon="undo"
                        color="green"
                        content="Restore"
                        onClick={() => handleRestore(project.id)}
                        size="small"
                      />
                      <Button
                        icon="trash"
                        color="red"
                        content="Delete"
                        onClick={() => handleDelete(project.id)}
                        size="small"
                        className="delete-btn"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}

          <Confirm
            open={confirmOpen}
            content="Are you sure you want to permanently delete this project? This action cannot be undone."
            onCancel={() => setConfirmOpen(false)}
            onConfirm={confirmDelete}
          />

          <Modal
            open={detailsModalOpen}
            onClose={() => setDetailsModalOpen(false)}
            size="small"
            className="admin-details-modal"
            closeIcon
          >
            <Modal.Header>Project Details</Modal.Header>
            <Modal.Content>
              {selectedProject && (
                <div>
                  <p><strong>Title:</strong> {selectedProject.projectTitle}</p>
                  <p><strong>Description:</strong> {selectedProject.description}</p>
                  <p><strong>Owner:</strong> {selectedProject.projectOwner}</p>
                  <p><strong>Start Date:</strong> {selectedProject.startDate ? new Date(selectedProject.startDate).toLocaleDateString() : "N/A"}</p>
                  <p><strong>Deleted Date:</strong> {selectedProject.deletedDate ? format(new Date(selectedProject.deletedDate), 'MMM dd, yyyy') : "N/A"}</p>
                  <p><strong>Tickets:</strong> {selectedProject.ticketCount || 0}</p>
                </div>
              )}
            </Modal.Content>
            <Modal.Actions>
              <Button onClick={() => setDetailsModalOpen(false)}>Close</Button>
              {selectedProject && (
                <Button as={Link} to={`/projects/${selectedProject.id}`} primary>
                  View Full Project
                </Button>
              )}
            </Modal.Actions>
          </Modal>
        </Segment>
      )}
    </div>
  );
});