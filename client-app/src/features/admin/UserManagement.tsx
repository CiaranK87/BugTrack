import { observer } from "mobx-react-lite";
import { useEffect } from "react";
import { Header, Segment, Table, Dropdown, Label, Icon, Loader } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { toast } from "react-toastify";

export default observer(function UserManagement() {
  const { userStore } = useStore();
  const { users, loadingUserList: loading, updateUserRole, updatingUserRole } = userStore;

  useEffect(() => {
    userStore.loadUsers();
  }, [userStore]);

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
      default: return "grey";
    }
  };

  const roleOptions = [
    { key: "user", text: "User", value: "User" },
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
      <Header as="h2" icon="users" content="User Management" subheader="Manage global user roles" />
      
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
                <Dropdown
                  text="Change Role"
                  icon="edit"
                  floating
                  labeled
                  button
                  className="icon"
                  loading={updatingUserRole}
                  disabled={updatingUserRole}
                >
                  <Dropdown.Menu>
                    {roleOptions.map((option) => (
                      <Dropdown.Item
                        key={option.key}
                        active={user.globalRole === option.value}
                        onClick={() => handleRoleChange(user.id, option.value)}
                        disabled={user.globalRole === option.value}
                      >
                        {option.text}
                      </Dropdown.Item>
                    ))}
                  </Dropdown.Menu>
                </Dropdown>
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
    </Segment>
  );
});