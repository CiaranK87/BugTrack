import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Confirm, Modal, Icon, Label, Input } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import { Link } from "react-router-dom";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { format } from "date-fns";

export default observer(function ProjectManagement() {
  const { projectStore } = useStore();
  const { projectsByStartDate, loadingInitial, deleteProject, restoreProject } = projectStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteProjectId, setDeleteProjectId] = useState("");
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedProject, setSelectedProject] = useState<any>(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    projectStore.loadProjects();
  }, [projectStore]);

  const handleDelete = (projectId: string) => {
    setDeleteProjectId(projectId);
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    deleteProject(deleteProjectId)
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

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'green';
      case 'Cancelled': return 'grey';
      default: return 'grey';
    }
  };

  const activeProjectsList = projectsByStartDate.filter(p => !p.isDeleted);

  // Filter projects based on search term
  const filteredProjects = activeProjectsList.filter(project =>
    project.projectTitle.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (project.projectOwner && project.projectOwner.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (project.description && project.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (project.isCancelled ? 'cancelled' : 'active').includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loadingInitial ? (
        <LoadingComponent content="Loading projects..." />
      ) : (
        <Segment className="admin-project-management">
          <div className="admin-project-controls" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="folder open" content="Project Management" subheader="Manage all projects" />
            <Input
              icon="search"
              placeholder="Search projects..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="admin-project-search"
              style={{ width: '250px' }}
            />
          </div>

          {filteredProjects.length === 0 ? (
            <Segment placeholder>
              <Header icon>
                <Icon name="folder open" />
              </Header>
              <p>No projects found</p>
            </Segment>
          ) : (
            <>
              <Table celled className="project-table-desktop">
                <Table.Header>
                  <Table.Row>
                    <Table.HeaderCell>Project Title</Table.HeaderCell>
                    <Table.HeaderCell>Owner</Table.HeaderCell>
                    <Table.HeaderCell>Start Date</Table.HeaderCell>
                    <Table.HeaderCell>Status</Table.HeaderCell>
                    <Table.HeaderCell>Tickets</Table.HeaderCell>
                    <Table.HeaderCell>Actions</Table.HeaderCell>
                  </Table.Row>
                </Table.Header>
                <Table.Body>
                  {filteredProjects.map((project) => (
                    <Table.Row key={project.id}>
                      <Table.Cell>
                        <Header as="h4">
                          {project.projectTitle}
                        </Header>
                      </Table.Cell>
                      <Table.Cell>{project.projectOwner || "Unknown"}</Table.Cell>
                      <Table.Cell>{project.startDate ? new Date(project.startDate).toLocaleDateString() : "N/A"}</Table.Cell>
                      <Table.Cell>
                        <Label color={getStatusColor(project.isCancelled ? 'Cancelled' : 'Active')} size="small">
                          {project.isCancelled ? 'Cancelled' : 'Active'}
                        </Label>
                      </Table.Cell>
                      <Table.Cell>{project.ticketCount || 0}</Table.Cell>
                      <Table.Cell>
                        <Button.Group>
                          <Button
                            icon="eye"
                            color="blue"
                            onClick={() => handleViewDetails(project)}
                            title="View Details"
                          />
                          {project.isDeleted ? (
                            <Button
                              icon="undo"
                              color="green"
                              onClick={() => handleRestore(project.id)}
                              title="Restore Project"
                            />
                          ) : (
                            <Button
                              icon="trash"
                              color="red"
                              onClick={() => handleDelete(project.id)}
                              title="Delete Project"
                            />
                          )}
                        </Button.Group>
                      </Table.Cell>
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>

              {/* Mobile Card View */}
              <div className="project-cards-mobile">
                {filteredProjects.map((project) => (
                  <div key={project.id} className="project-mobile-card">
                    <div className="project-card-header">
                      <Header as="h4">
                        <Icon name="folder" />
                        <Header.Content>
                          {project.projectTitle}
                          <Header.Subheader>{project.projectOwner || "Unknown"}</Header.Subheader>
                        </Header.Content>
                      </Header>
                      <Label color={getStatusColor(project.isCancelled ? 'Cancelled' : 'Active')} size="small" className="project-status-label">
                        {project.isCancelled ? 'Cancelled' : 'Active'}
                      </Label>
                    </div>

                    <div className="project-card-content">
                      <div className="project-detail-item">
                        <span className="detail-label">Start Date:</span>
                        <span className="detail-value">{project.startDate ? new Date(project.startDate).toLocaleDateString() : "N/A"}</span>
                      </div>
                      <div className="project-detail-item">
                        <span className="detail-label">Tickets:</span>
                        <span className="detail-value">{project.ticketCount || 0}</span>
                      </div>
                      <div className="project-detail-item">
                        <span className="detail-label">Description:</span>
                        <div className="detail-value description-truncate">{project.description || "No description"}</div>
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
                      {project.isDeleted ? (
                        <Button
                          icon="undo"
                          color="green"
                          content="Restore"
                          onClick={() => handleRestore(project.id)}
                          size="small"
                        />
                      ) : (
                        <Button
                          icon="trash"
                          color="red"
                          content="Delete"
                          onClick={() => handleDelete(project.id)}
                          size="small"
                        />
                      )}
                      <Button
                        as={Link}
                        to={`/projects/${project.id}`}
                        primary
                        content="Full Page"
                        icon="external alternate"
                        size="small"
                        className="full-page-btn"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}

          <Confirm
            open={confirmOpen}
            content="Are you sure you want to delete this project? This will move it to the deleted projects list."
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
                  <p><strong>Status:</strong>
                    <Label color={getStatusColor(selectedProject.isCancelled ? 'Cancelled' : 'Active')} size="small" style={{ marginLeft: '5px' }}>
                      {selectedProject.isCancelled ? 'Cancelled' : 'Active'}
                    </Label>
                  </p>
                  <p><strong>Tickets:</strong> {selectedProject.ticketCount || 0}</p>
                  {selectedProject.isDeleted && (
                    <p><strong>Deleted Date:</strong> {selectedProject.deletedDate ? format(new Date(selectedProject.deletedDate), 'MMM dd, yyyy') : "N/A"}</p>
                  )}
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