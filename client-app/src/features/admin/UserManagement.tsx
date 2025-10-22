import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import { Header, Segment, Table, Dropdown, Label, Icon, Loader, Button, Confirm } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";
import AdminRegisterForm from "./AdminRegisterForm";
import EditUserForm from "./EditUserForm";

export default observer(function UserManagement() {
  const { userStore, modalStore } = useStore();
  const { users, loadingUserList: loading, updateUserRole, updatingUserRole, deleteUser, user: currentUser } = userStore;
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [deleteUserId, setDeleteUserId] = useState("");

  useEffect(() => {
    userStore.loadUsers();
  }, [userStore]);

  const handleEdit = (userId: string) => {
    modalStore.openModal(<EditUserForm userId={userId} />);
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

  const roleOptions = [
    { key: "user", text: "User", value: "User" },
    { key: "developer", text: "Developer", value: "Developer" },
    { key: "projectmanager", text: "Project Manager", value: "ProjectManager" },
    { key: "admin", text: "Admin", value: "Admin" }
  ];

  if (loading) {
    return (
      <Segment>
        <Loader active>Loading users...</Loader>
      </Segment>
    );
  }

  return (
    <Segment>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
        <Header as="h2" icon="users" content="User Management" subheader="Manage global user roles" />
        <Button
          positive
          content="Add New User"
          icon="user plus"
          onClick={() => modalStore.openModal(<AdminRegisterForm />)}
          size="small"
        />
      </div>
      
      <Table celled>
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
          {users.map((user) => (
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
      
      <Confirm
        open={confirmOpen}
        content="Are you sure you want to delete this user? This action cannot be undone."
        onCancel={() => setConfirmOpen(false)}
        onConfirm={confirmDelete}
      />
    </Segment>
  );
});