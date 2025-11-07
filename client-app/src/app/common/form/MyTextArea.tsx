import { useField } from "formik";
import { Form, Label } from "semantic-ui-react";

interface Props {
  placeholder?: string;
  name: string;
  rows?: number;
  label?: string;
}

export default function MyTextArea(props: Props) {
  const [field, meta] = useField(props.name);
  return (
    <Form.Field error={meta.touched && !!meta.error}>
      {props.label && <label>{props.label}</label>}
      <textarea
        {...field}
        placeholder={props.placeholder}
        rows={props.rows}
      />
      {meta.touched && meta.error ? (
        <Label basic color="red">
          {meta.error}
        </Label>
      ) : null}
    </Form.Field>
  );
}
