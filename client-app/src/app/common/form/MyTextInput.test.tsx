import { render, screen } from '@testing-library/react';
import { Formik, Form } from 'formik';
import MyTextInput from './MyTextInput';

test('renders MyTextInput with placeholder and label', () => {
  render(
    <Formik initialValues={{ testField: '' }} onSubmit={() => {}}>
      <Form>
        <MyTextInput placeholder="Enter text" name="testField" label="Test Label" />
      </Form>
    </Formik>
  );
  
  // Check if the label is rendered
  expect(screen.getByText('Test Label')).toBeInTheDocument();
  
  // Check if the input with placeholder is rendered
  expect(screen.getByPlaceholderText('Enter text')).toBeInTheDocument();
});

test('renders MyTextInput without label', () => {
  render(
    <Formik initialValues={{ testField: '' }} onSubmit={() => {}}>
      <Form>
        <MyTextInput placeholder="Enter text" name="testField" />
      </Form>
    </Formik>
  );
  
  // Check that the label is not rendered
  expect(screen.queryByText('Test Label')).not.toBeInTheDocument();
  
  // Check if the input with placeholder is rendered
  expect(screen.getByPlaceholderText('Enter text')).toBeInTheDocument();
});

test('renders MyTextInput with error message', async () => {
  render(
    <Formik
      initialValues={{ testField: '' }}
      initialErrors={{ testField: 'This field is required' }}
      initialTouched={{ testField: true }}
      onSubmit={() => {}}
    >
      <Form>
        <MyTextInput placeholder="Enter text" name="testField" label="Test Label" />
      </Form>
    </Formik>
  );
  
  // Check if the error message is rendered
  expect(screen.getByText('This field is required')).toBeInTheDocument();
});