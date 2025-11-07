import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Button, Loader, Confirm, Modal, Icon } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import { Link } from "react-router-dom";

export default observer(function DeletedProjectsManagement() {
  const { projectStore } = useStore();
  const { projectsByStartDate, loadingInitial, adminDeleteProject } = projectStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteProjectId, setDeleteProjectId] = useState("");
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedProject, setSelectedProject] = useState<any>(null);

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
        toast.success("Project permanently deleted successfully");
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

  const handleRestore = (_projectId: string) => {
    // We'll implement this later
    toast.info("Restore functionality coming soon");
  };

  const handleViewDetails = (project: any) => {
    setSelectedProject(project);
    setDetailsModalOpen(true);
  };

  const deletedProjects = projectsByStartDate.filter(p => p.isDeleted);

  if (loadingInitial) {
    return (
      <Segment>
        <Loader active>Loading deleted projects...</Loader>
      </Segment>
    );
  }

  return (
    <Segment>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
        <Header as="h2" icon="trash" content="Deleted Projects" subheader="Manage permanently deleted projects" />
      </div>
      
      {deletedProjects.length === 0 ? (
        <Segment placeholder>
          <Header icon>
            <Icon name="trash" />
          </Header>
          <p>No deleted projects found</p>
        </Segment>
      ) : (
        <Table celled>
          <Table.Header>
            <Table.Row>
              <Table.HeaderCell>Project Title</Table.HeaderCell>
              <Table.HeaderCell>Owner</Table.HeaderCell>
              <Table.HeaderCell>Start Date</Table.HeaderCell>
              <Table.HeaderCell>Tickets</Table.HeaderCell>
              <Table.HeaderCell>Actions</Table.HeaderCell>
            </Table.Row>
          </Table.Header>
          <Table.Body>
            {deletedProjects.map((project) => (
              <Table.Row key={project.id}>
                <Table.Cell>
                  <Header as="h4">
                    {project.projectTitle}
                  </Header>
                </Table.Cell>
                <Table.Cell>{project.projectOwner || "Unknown"}</Table.Cell>
                <Table.Cell>{project.startDate ? new Date(project.startDate).toLocaleDateString() : "N/A"}</Table.Cell>
                <Table.Cell>{project.ticketCount || 0}</Table.Cell>
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
                      icon="delete"
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
      >
        <Modal.Header>Project Details</Modal.Header>
        <Modal.Content>
          {selectedProject && (
            <div>
              <p><strong>Title:</strong> {selectedProject.projectTitle}</p>
              <p><strong>Description:</strong> {selectedProject.description}</p>
              <p><strong>Owner:</strong> {selectedProject.projectOwner}</p>
              <p><strong>Start Date:</strong> {selectedProject.startDate ? new Date(selectedProject.startDate).toLocaleDateString() : "N/A"}</p>
              <p><strong>Deleted Date:</strong> {selectedProject.deletedDate ? new Date(selectedProject.deletedDate).toLocaleDateString() : "N/A"}</p>
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
  );
});