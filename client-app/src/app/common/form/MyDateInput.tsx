import { useField } from "formik";
import { Form, Label } from "semantic-ui-react";
import DatePicker, { ReactDatePickerProps } from "react-datepicker";

interface Props extends Partial<ReactDatePickerProps> {
  name: string;
  label?: string;
}

export default function MyDateInput(props: Props) {
  const [field, meta, helpers] = useField(props.name);
  return (
    <Form.Field error={meta.touched && !!meta.error}>
      {props.label && <label style={{
        color: '#4DB6AC',
        fontSize: '0.9em',
        fontWeight: 'bold',
        marginBottom: '1px',
        marginLeft: '4px'
      }}>{props.label}</label>}
      <DatePicker
        {...field}
        {...props}
        selected={(field.value && new Date(field.value)) || null}
        onChange={(value) => helpers.setValue(value)}
      />
      {meta.touched && meta.error ? (
        <Label basic color="red">
          {meta.error}
        </Label>
      ) : null}
    </Form.Field>
  );
}
