import { ErrorMessage, Formik, Form } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Segment } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import * as Yup from "yup";
import ValidationError from "../errors/ValidationError";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import LoadingComponent from "../../app/layout/LoadingComponent";

export default observer(function EditUserForm({ userId: propUserId }: { userId?: string }) {
  const { userStore } = useStore();
  const navigate = useNavigate();
  const { id: urlId, userId: urlUserId } = useParams<{ id: string; userId: string }>();
  const userId = propUserId || urlId || urlUserId;

  useEffect(() => {
    if (userId && userStore.users.length === 0) {
      userStore.loadUsers();
    }
  }, [userId, userStore]);

  const user = userStore.users.find(u => u.id === userId);

  if (!user && userStore.loadingUserList) return <LoadingComponent content="Loading user..." />;
  if (!user) return <Segment>User not found</Segment>;

  return (
    <Segment clearing className="admin-user-form">
      <Header content="Edit User" sub color="teal" />
      <div style={{ marginBottom: '20px' }}></div>
      <Formik
        initialValues={{
          displayName: user.displayName,
          username: user.username,
          email: user.email,
          jobTitle: user.jobTitle || "",
          bio: user.bio || "",
          error: null
        }}
        onSubmit={(values, { setErrors }) => {
          const updateData = {
            displayName: values.displayName,
            username: values.username,
            email: values.email,
            jobTitle: values.jobTitle,
            bio: values.bio
          };
          return userStore.updateUser(userId!, updateData)
            .then(() => {
              navigate("/admin/users");
            })
            .catch((error) => setErrors({ error }));
        }}
        validationSchema={Yup.object({
          displayName: Yup.string().required("Display name is required"),
          username: Yup.string().required("Username is required"),
          email: Yup.string().email("Invalid email").required("Email is required"),
        })}
      >
        {({ handleSubmit, isSubmitting, errors, isValid, dirty }) => (
          <Form className="ui form error" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput label="Display Name" name="displayName" placeholder="Display Name" />
            <MyTextInput label="Username" name="username" placeholder="Username" />
            <MyTextInput label="Email Address" name="email" placeholder="Email" />
            <MyTextInput label="Job Title" name="jobTitle" placeholder="Job Title" />
            <MyTextInput label="Bio" name="bio" placeholder="Bio" />
            <ErrorMessage
              name="error"
              render={() => <ValidationError errors={errors.error as unknown as string[]} />}
            />
            <div style={{ marginTop: '20px', overflow: 'hidden' }}>
              <Button
                disabled={!isValid || !dirty || isSubmitting}
                loading={isSubmitting}
                positive
                content="Update"
                type="submit"
                floated="right"
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
