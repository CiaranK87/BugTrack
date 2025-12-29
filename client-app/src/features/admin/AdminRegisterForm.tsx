import { ErrorMessage, Formik, Form } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Segment, Dropdown } from "semantic-ui-react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import * as Yup from "yup";
import ValidationError from "../errors/ValidationError";
import { Link, useNavigate } from "react-router-dom";

export default observer(function AdminRegisterForm() {
  const { userStore } = useStore();
  const navigate = useNavigate();
  const [selectedRole, setSelectedRole] = useState("User");

  const roleOptions = [
    { key: "user", text: "User", value: "User" },
    { key: "developer", text: "Developer", value: "Developer" },
    { key: "projectmanager", text: "Project Manager", value: "ProjectManager" },
    { key: "admin", text: "Admin", value: "Admin" }
  ];

  return (
    <Segment clearing className="admin-user-form">
      <Header content="Add New User" sub color="teal" />
      <div style={{ marginBottom: '20px' }}></div>
      <Formik
        initialValues={{ displayName: "", username: "", email: "", password: "", jobTitle: "", error: null }}
        onSubmit={(values, { setErrors }) => {
          const formData = {
            displayName: values.displayName,
            username: values.username,
            email: values.email,
            password: values.password,
            jobTitle: values.jobTitle,
            role: selectedRole
          };
          return userStore.adminRegister(formData)
            .then(() => {
              navigate("/admin/users");
            })
            .catch((error) => setErrors({ error }));
        }}
        validationSchema={Yup.object({
          displayName: Yup.string().required("Display name is required"),
          username: Yup.string().required("Username is required"),
          email: Yup.string().email("Invalid email").required("Email is required"),
          password: Yup.string().required("Password is required"),
          jobTitle: Yup.string(),
        })}
      >
        {({ handleSubmit, isSubmitting, errors, isValid, dirty }) => (
          <Form className="ui form error" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput label="Display Name" name="displayName" placeholder="Display Name" />
            <MyTextInput label="Username" name="username" placeholder="Username" />
            <MyTextInput label="Email Address" name="email" placeholder="Email" />
            <MyTextInput label="Password" name="password" placeholder="Password" type="password" />
            <MyTextInput label="Job Title" name="jobTitle" placeholder="Job Title" />
            <SemanticForm.Field>
              <label style={{ color: '#4DB6AC', fontSize: '0.9em', fontWeight: 'bold' }}>Initial Role</label>
              <Dropdown
                placeholder="Select a role"
                fluid
                selection
                options={roleOptions}
                value={selectedRole}
                onChange={(_, data) => setSelectedRole(data.value as string)}
              />
            </SemanticForm.Field>
            <ErrorMessage
              name="error"
              render={() => <ValidationError errors={errors.error as unknown as string[]} />}
            />
            <div style={{ marginTop: '20px', overflow: 'hidden' }}>
              <Button
                disabled={isSubmitting || !dirty || !isValid}
                loading={isSubmitting}
                floated="right"
                positive
                type="submit"
                content="Create User"
              />
              <Button
                as={Link}
                to="/admin/users"
                floated="right"
                type="button"
                content="Cancel"
              />
            </div>
          </Form>
        )}
      </Formik>
    </Segment>
  );
});
