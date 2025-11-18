import { Form, Formik } from "formik";
import { useState } from "react";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";
import * as Yup from "yup";
import ValidationError from "../errors/ValidationError";
import { toast } from "react-toastify";

export default observer(function ChangePasswordForm() {
  const { userStore } = useStore();
  const [submitError, setSubmitError] = useState<string[] | null>(null);

  const validationSchema = Yup.object({
    currentPassword: Yup.string().required("Current password is required"),
    newPassword: Yup.string()
      .required("New password is required")
      .min(6, "Password must be at least 6 characters")
      .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/, "Password must contain at least one uppercase letter, one lowercase letter, and one number"),
    confirmNewPassword: Yup.string()
      .required("Please confirm your new password")
      .oneOf([Yup.ref('newPassword')], 'Passwords must match'),
  });

  return (
    <Formik
      initialValues={{ currentPassword: "", newPassword: "", confirmNewPassword: "" }}
      validationSchema={validationSchema}
      onSubmit={async (values, { setSubmitting }) => {
        setSubmitError(null);
        try {
          await userStore.changePassword(values);
          toast.success("Password changed successfully");
          setSubmitting(false);
        } catch (error: any) {
          // Handle different error formats
          let errorMessages: string[] = [];
          
          if (Array.isArray(error)) {
            errorMessages = error;
          } else if (typeof error === 'string') {
            errorMessages = [error];
          } else if (error?.message) {
            errorMessages = [error.message];
          } else if (error?.response?.data?.message) {
            errorMessages = [error.response.data.message];
          } else {
            errorMessages = ['An unexpected error occurred while changing password'];
          }
          
          setSubmitError(errorMessages);
          setSubmitting(false);
        }
      }}
    >
      {({ handleSubmit, isSubmitting, isValid, dirty }) => (
        <Form className="ui form error" onSubmit={handleSubmit} autoComplete="off">
          <Header as="h2" content="Change Password" color="teal" textAlign="center" />
          <MyTextInput name="currentPassword" placeholder="Current Password" type="password" />
          <MyTextInput name="newPassword" placeholder="New Password" type="password" />
          <MyTextInput name="confirmNewPassword" placeholder="Confirm New Password" type="password" />
          {submitError && <ValidationError errors={submitError} />}
          <Button
            disabled={!isValid || !dirty || isSubmitting}
            loading={isSubmitting}
            positive
            content="Change Password"
            type="submit"
            fluid
          />
        </Form>
      )}
    </Formik>
  );
});