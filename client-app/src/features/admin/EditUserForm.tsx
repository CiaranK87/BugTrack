import { ErrorMessage, Formik } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Segment, Form as SemanticForm } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import * as Yup from "yup";
import ValidationError from "../errors/ValidationError";

export default observer(function EditUserForm({ userId, onSuccess }: { userId: string; onSuccess?: () => void }) {
  const { userStore, modalStore } = useStore();
  const user = userStore.users.find(u => u.id === userId);

  if (!user) return null;

  return (
    <Segment clearing>
      <Header content="Edit User" sub color="teal" />
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
          return userStore.updateUser(userId, updateData)
            .then(() => {
              modalStore.closeModal();
              if (onSuccess) onSuccess();
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
          <SemanticForm className="ui form error" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="displayName" placeholder="Display Name" />
            <MyTextInput name="username" placeholder="Username" />
            <MyTextInput name="email" placeholder="Email" />
            <MyTextInput name="jobTitle" placeholder="Job Title" />
            <MyTextInput name="bio" placeholder="Bio" />
            <ErrorMessage
              name="error"
              render={() => <ValidationError errors={errors.error as unknown as string[]} />}
            />
            <Button
              disabled={!isValid || !dirty || isSubmitting}
              loading={isSubmitting}
              positive
              content="Update User"
              type="submit"
              fluid
            />
          </SemanticForm>
        )}
      </Formik>
    </Segment>
  );
});