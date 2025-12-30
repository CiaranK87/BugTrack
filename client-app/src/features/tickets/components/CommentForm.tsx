import React, { useState, useRef } from 'react';
import { Formik, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Button, Header, Icon, Segment, Label } from 'semantic-ui-react';
import MyTextArea from '../../../app/common/form/MyTextArea';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import { logger } from '../../../app/utils/logger';

interface Props {
  ticketId: string;
  parentCommentId?: string;
  isReply?: boolean;
}

export default observer(function CommentForm({ ticketId, parentCommentId, isReply = false }: Props) {
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
      await createComment(ticketId, values.content, selectedFiles, parentCommentId);
      resetForm();
      setSelectedFiles([]);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }

      if (isReply && parentCommentId) {
        window.dispatchEvent(new CustomEvent('closeReplyForm', { detail: { commentId: parentCommentId } }));
      }
    } catch (error) {
      logger.error('Failed to create comment', error);
    }
  };

  return (
    <Segment clearing>
      <Header content={isReply ? 'Add Reply' : 'Add Comment'} sub color='teal' />
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
            />

            <div className="comment-form-footer">
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