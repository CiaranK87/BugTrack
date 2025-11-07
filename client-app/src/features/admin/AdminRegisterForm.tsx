import { ErrorMessage, Formik } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Segment, Dropdown } from "semantic-ui-react";
import { Form as SemanticForm } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import * as Yup from "yup";
import ValidationError from "../errors/ValidationError";

export default observer(function AdminRegisterForm() {
  const { userStore, modalStore } = useStore();
  const [selectedRole, setSelectedRole] = useState("User");

  const roleOptions = [
    { key: "user", text: "User", value: "User" },
    { key: "developer", text: "Developer", value: "Developer" },
    { key: "projectmanager", text: "Project Manager", value: "ProjectManager" },
    { key: "admin", text: "Admin", value: "Admin" }
  ];

  return (
    <Segment clearing>
      <Header content="Add New User" sub color="teal" />
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
              modalStore.closeModal();
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
          <SemanticForm className="ui form error" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="displayName" placeholder="Display Name" />
            <MyTextInput name="username" placeholder="Username" />
            <MyTextInput name="email" placeholder="Email" />
            <MyTextInput name="password" placeholder="Password" type="password" />
            <MyTextInput name="jobTitle" placeholder="Job Title" />
            <SemanticForm.Field>
              <label>Initial Role</label>
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
            <Button
              disabled={!isValid || !dirty || isSubmitting}
              loading={isSubmitting}
              positive
              content="Create User"
              type="submit"
              fluid
            />
          </SemanticForm>
        )}
      </Formik>
    </Segment>
  );
});