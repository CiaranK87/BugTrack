import React, { useState, useRef } from 'react';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Button, Header, Icon, Segment, Label } from 'semantic-ui-react';
import MyTextArea from '../../../app/common/form/MyTextArea';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';

interface Props {
  ticketId: string;
}

export default observer(function CommentForm({ ticketId }: Props) {
  const { commentStore } = useStore();
  const { createComment, loading } = commentStore;
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const validationSchema = Yup.object({
    content: Yup.string().required('Comment content is required'),
  });

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
      const filesArray = Array.from(event.target.files);
      setSelectedFiles(prevFiles => [...prevFiles, ...filesArray]);
    }
  };

  const removeFile = (index: number) => {
    setSelectedFiles(prevFiles => prevFiles.filter((_, i) => i !== index));
  };

  const handleFormSubmit = async (values: { content: string }, { resetForm }: { resetForm: () => void }) => {
    try {
      await createComment(ticketId, values.content, selectedFiles);
      resetForm();
      setSelectedFiles([]);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    } catch (error) {
      console.error('Failed to create comment:', error);
    }
  };

  return (
    <Segment clearing>
      <Header content='Add Comment' sub color='teal' />
      <Formik
        initialValues={{ content: '' }}
        validationSchema={validationSchema}
        onSubmit={handleFormSubmit}
      >
        {({ isSubmitting, isValid }) => (
          <Form className='ui form'>
            <MyTextArea
              name='content'
              placeholder='Write your comment here...'
              rows={3}
              label=''
            />
            <ErrorMessage
              name='content'
              render={() => <Label basic color='red' pointing content='Comment content is required' />}
            />
            
            <div style={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              margin: '10px 0'
            }}>
              <div style={{ display: 'flex', alignItems: 'center' }}>
                <input
                  type='file'
                  ref={fileInputRef}
                  multiple
                  onChange={handleFileChange}
                  style={{ display: 'none' }}
                />
                <Button
                  type='button'
                  content='Attach Files'
                  labelPosition='left'
                  icon='attach'
                  onClick={() => fileInputRef.current?.click()}
                  basic
                />
              </div>

              <Button
                loading={loading || isSubmitting}
                disabled={!isValid || isSubmitting}
                positive
                type='submit'
                content='Submit'
              />
            </div>

            {selectedFiles.length > 0 && (
              <div style={{ margin: '10px 0' }}>
                <Header sub content='Attachments' />
                {selectedFiles.map((file, index) => (
                  <Label key={index} style={{ margin: '2px' }}>
                    {file.name}
                    <Icon name='delete' onClick={() => removeFile(index)} />
                  </Label>
                ))}
              </div>
            )}
          </Form>
        )}
      </Formik>
    </Segment>
  );
});