import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Confirm, Modal, Icon, Label } from "semantic-ui-react";
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

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loadingInitial ? (
        <LoadingComponent content="Loading projects..." />
      ) : (
        <Segment>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="folder open" content="Project Management" subheader="Manage all projects" />
          </div>
          
          {activeProjectsList.length === 0 ? (
            <Segment placeholder>
              <Header icon>
                <Icon name="folder open" />
              </Header>
              <p>No projects found</p>
            </Segment>
          ) : (
            <Table celled>
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
                {activeProjectsList.map((project) => (
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