import { ErrorMessage, Form, Formik, useFormikContext } from "formik";
import { useEffect, useState } from "react";
import MyTextInput from "../../app/common/form/MyTextInput";
import { Button, Header, Label } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { observer } from "mobx-react-lite";

function ColdStartHint() {
  const { isSubmitting } = useFormikContext();
  const [show, setShow] = useState(false);

  useEffect(() => {
    if (!isSubmitting) {
      setShow(false);
      return;
    }
    const timer = setTimeout(() => setShow(true), 5000);
    return () => clearTimeout(timer);
  }, [isSubmitting]);

  if (!show) return null;

  return (
    <p className="cold-start-hint">
      The server may be waking up after inactivity. First requests can take up to 30 seconds.
    </p>
  );
}

export default observer(function LoginForm() {
  const { userStore } = useStore();

  return (
    <Formik
      initialValues={{ email: "", password: "", error: null }}
      onSubmit={(values, { setErrors }) => userStore.login(values).catch((error) => {
        if (error.response) {
          setErrors({ error: "Invalid email or password" });
        }
      })}
    >
      {({ handleSubmit, isSubmitting, errors }) => (
        <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
          <Header as="h2" content="Login to BugTrack" color="teal" textAlign="center" />
          <MyTextInput name="email" placeholder="Email" />
          <MyTextInput name="password" placeholder="Password" type="password" />
          <ErrorMessage
            name="error"
            render={() => <Label style={{ marginBottom: 10 }} basic color="red" content={errors.error} />}
          />
          <ColdStartHint />
          <Button loading={isSubmitting} positive content="Login" type="submit" fluid />
        </Form>
      )}
    </Formik>
  );
});
