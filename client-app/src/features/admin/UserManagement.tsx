import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Dropdown, Label, Icon, Button, Confirm, Tab, Input } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import LoadingComponent from "../../app/layout/LoadingComponent";
import DeletedProjectsManagement from "./DeletedProjectsManagement";
import DeletedTicketsManagement from "./DeletedTicketsManagement";
import TicketManagement from "./TicketManagement";
import ProjectManagement from "./ProjectManagement";
import { Link, useNavigate } from "react-router-dom";

export default observer(function UserManagement() {
  const { userStore } = useStore();
  const navigate = useNavigate();
  const { users, loadingUserList: loading, updateUserRole, updatingUserRole, deleteUser, user: currentUser } = userStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteUserId, setDeleteUserId] = useState("");
  const [activeTab, setActiveTab] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    userStore.loadUsers();
  }, [userStore]);

  const handleEdit = (userId: string) => {
    navigate(`/admin/users/edit/${userId}`);
  };

  const handleDelete = (userId: string) => {
    setDeleteUserId(userId);
    setConfirmOpen(true);
  };

  const confirmDelete = () => {
    deleteUser(deleteUserId)
      .then(() => {
        toast.success("User deleted successfully");
      })
      .catch(() => {
        toast.error("Failed to delete user");
      })
      .finally(() => {
        setConfirmOpen(false);
        setDeleteUserId("");
      });
  };

  const handleRoleChange = async (userId: string, newRole: string) => {
    try {
      await updateUserRole(userId, newRole);
      toast.success("User role updated successfully");
    } catch (error) {
      toast.error("Failed to update user role");
    }
  };

  const getRoleColor = (role: string) => {
    switch (role) {
      case "Admin": return "red";
      case "ProjectManager": return "blue";
      case "Developer": return "green";
      default: return "grey";
    }
  };

  // Filter users based on search term
  const filteredUsers = users.filter(user =>
    user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.displayName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (user.jobTitle && user.jobTitle.toLowerCase().includes(searchTerm.toLowerCase())) ||
    user.globalRole.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const roleOptions = [
    { key: "user", text: "User", value: "User" },
    { key: "developer", text: "Developer", value: "Developer" },
    { key: "projectmanager", text: "Project Manager", value: "ProjectManager" },
    { key: "admin", text: "Admin", value: "Admin" }
  ];

  if (loading) {
    return <LoadingComponent content="Loading users..." />;
  }

  const panes = [
    {
      menuItem: 'User Management',
      render: () => (
        <Segment attached="bottom" className="admin-user-management">
          <div className="admin-user-controls" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
            <Header as="h2" icon="users" content="User Management" subheader="Manage global user roles" />
            <div className="admin-user-actions" style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
              <Input
                icon="search"
                placeholder="Search users..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="admin-user-search"
                style={{ width: '250px' }}
              />
              <Button
                positive
                content="Add New"
                icon="user plus"
                as={Link}
                to="/admin/users/create"
                size="small"
                className="admin-add-user-btn"
              />
            </div>
          </div>

          <Table celled className="user-table-desktop">
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell>Username</Table.HeaderCell>
                <Table.HeaderCell>Display Name</Table.HeaderCell>
                <Table.HeaderCell>Email</Table.HeaderCell>
                <Table.HeaderCell>Global Role</Table.HeaderCell>
                <Table.HeaderCell>Job Title</Table.HeaderCell>
                <Table.HeaderCell>Actions</Table.HeaderCell>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {filteredUsers.map((user) => (
                <Table.Row key={user.id}>
                  <Table.Cell>
                    <Header as="h4" image>
                      <Icon name="user" />
                      <Header.Content>{user.username}</Header.Content>
                    </Header>
                  </Table.Cell>
                  <Table.Cell>{user.displayName}</Table.Cell>
                  <Table.Cell>{user.email}</Table.Cell>
                  <Table.Cell>
                    <Label color={getRoleColor(user.globalRole)} size="large">
                      {user.globalRole}
                    </Label>
                  </Table.Cell>
                  <Table.Cell>{user.jobTitle || "N/A"}</Table.Cell>
                  <Table.Cell>
                    <Button.Group>
                      <Button
                        icon="edit"
                        color="blue"
                        onClick={() => handleEdit(user.id)}
                        disabled={user.globalRole === "Admin" && user.username !== currentUser?.username}
                        title="Edit User"
                      />
                      <Button
                        icon="trash"
                        color="red"
                        onClick={() => handleDelete(user.id)}
                        disabled={user.globalRole === "Admin" || user.username === currentUser?.username}
                        title="Delete User"
                      />
                      <Dropdown
                        text="Role"
                        icon="user"
                        floating
                        labeled
                        button
                        className="icon"
                        loading={updatingUserRole}
                        disabled={updatingUserRole || (user.globalRole === "Admin" && user.username !== currentUser?.username)}
                      >
                        <Dropdown.Menu>
                          {roleOptions.map((option) => (
                            <Dropdown.Item
                              key={option.key}
                              active={user.globalRole === option.value}
                              onClick={() => handleRoleChange(user.id, option.value)}
                              disabled={user.globalRole === option.value || (user.globalRole === "Admin" && user.username !== currentUser?.username)}
                            >
                              {option.text}
                            </Dropdown.Item>
                          ))}
                        </Dropdown.Menu>
                      </Dropdown>
                    </Button.Group>
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>

          {/* Mobile Card View */}
          <div className="user-cards-mobile">
            {filteredUsers.map((user) => (
              <div key={user.id} className="user-mobile-card">
                <div className="user-card-header">
                  <Header as="h4">
                    <Icon name="user" />
                    <Header.Content>
                      {user.username}
                      <Header.Subheader>{user.displayName}</Header.Subheader>
                    </Header.Content>
                  </Header>
                  <Label color={getRoleColor(user.globalRole)} size="small" className="user-role-label">
                    {user.globalRole}
                  </Label>
                </div>

                <div className="user-card-content">
                  <div className="user-detail-item">
                    <span className="detail-label">Email:</span>
                    <span className="detail-value">{user.email}</span>
                  </div>
                  <div className="user-detail-item">
                    <span className="detail-label">Title:</span>
                    <span className="detail-value">{user.jobTitle || "N/A"}</span>
                  </div>
                </div>

                <div className="user-card-actions">
                  <Button
                    icon="edit"
                    color="blue"
                    content="Edit"
                    onClick={() => handleEdit(user.id)}
                    disabled={user.globalRole === "Admin" && user.username !== currentUser?.username}
                    size="small"
                  />
                  <Button
                    icon="trash"
                    color="red"
                    content="Delete"
                    onClick={() => handleDelete(user.id)}
                    disabled={user.globalRole === "Admin" || user.username === currentUser?.username}
                    size="small"
                  />
                  <Dropdown
                    text="Role"
                    icon="user"
                    floating
                    labeled
                    button
                    className="icon mobile-role-dropdown"
                    loading={updatingUserRole}
                    disabled={updatingUserRole || (user.globalRole === "Admin" && user.username !== currentUser?.username)}
                    size="small"
                  >
                    <Dropdown.Menu>
                      {roleOptions.map((option) => (
                        <Dropdown.Item
                          key={option.key}
                          active={user.globalRole === option.value}
                          onClick={() => handleRoleChange(user.id, option.value)}
                          disabled={user.globalRole === option.value || (user.globalRole === "Admin" && user.username !== currentUser?.username)}
                        >
                          {option.text}
                        </Dropdown.Item>
                      ))}
                    </Dropdown.Menu>
                  </Dropdown>
                </div>
              </div>
            ))}
            {filteredUsers.length === 0 && (
              <Segment textAlign="center" basic>
                <Icon name="search" size="large" />
                <p>No users found matching your search.</p>
              </Segment>
            )}
          </div>
        </Segment>
      )
    },
    {
      menuItem: 'Project Management',
      render: () => <ProjectManagement />
    },
    {
      menuItem: 'Ticket Management',
      render: () => <TicketManagement />
    },
    {
      menuItem: 'Deleted Projects',
      render: () => <DeletedProjectsManagement />
    },
    {
      menuItem: 'Deleted Tickets',
      render: () => <DeletedTicketsManagement />
    }
  ];

  return (
    <div style={{ minHeight: '100vh', position: 'relative' }}>
      {loading ? (
        <LoadingComponent content="Loading users..." />
      ) : (
        <div>
          <Tab
            menu={{ attached: true, tabular: true }}
            panes={panes}
            activeIndex={activeTab}
            onTabChange={(_, data) => setActiveTab(data.activeIndex as number)}
            className="admin-management-tabs"
          />

          <Confirm
            open={confirmOpen}
            content="Are you sure you want to delete this user? This action cannot be undone."
            onCancel={() => setConfirmOpen(false)}
            onConfirm={confirmDelete}
          />
        </div>
      )}
    </div>
  );
});