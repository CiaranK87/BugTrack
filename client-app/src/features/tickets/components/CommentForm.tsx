import React, { useState, useRef, useEffect } from 'react';
import { Formik, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { Button, Header, Icon, Segment, Label } from 'semantic-ui-react';
import MyTextArea from '../../../app/common/form/MyTextArea';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import { logger } from '../../../app/utils/logger';
import MentionSuggestions from '../../../app/common/mentions/MentionSuggestions';

interface Props {
  ticketId: string;
  parentCommentId?: string;
  isReply?: boolean;
}

export default observer(function CommentForm({ ticketId, parentCommentId, isReply = false }: Props) {
  const { commentStore, ticketStore } = useStore();
  const { createComment, loading } = commentStore;
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  
  const [showMentions, setShowMentions] = useState(false);
  const [mentionQuery, setMentionQuery] = useState('');
  const [mentionPosition, setMentionPosition] = useState({ top: 0, left: 0 });
  const [cursorPosition, setCursorPosition] = useState(0);
  const [atSymbolPosition, setAtSymbolPosition] = useState(0);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const setFieldValueRef = useRef<((field: string, value: any) => void) | null>(null);
  const isMentioningRef = useRef(false);

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

  const handleTextareaChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    const value = event.target.value;
    const cursorPos = event.target.selectionStart;
    
    setCursorPosition(cursorPos);
    
    const textBeforeCursor = value.substring(0, cursorPos);
    const lastAtIndex = textBeforeCursor.lastIndexOf('@');
    
    if (lastAtIndex !== -1) {
      const textAfterAt = textBeforeCursor.substring(lastAtIndex + 1);
      const hasSpaceAfterAt = textAfterAt.includes(' ');
      
      if (!hasSpaceAfterAt && textAfterAt.length >= 0) {
        setMentionQuery(textAfterAt);
        setAtSymbolPosition(lastAtIndex);
        
        if (textareaRef.current) {
          const lines = textBeforeCursor.split('\n');
          const lineHeight = 20;
          const charWidth = 8;
          const top = (lines.length - 1) * lineHeight + 40;
          const left = (lines[lines.length - 1].length * charWidth) + 20;
          
          setMentionPosition({ top, left });
          setShowMentions(true);
          isMentioningRef.current = true;
        }
      } else {
        setShowMentions(false);
        isMentioningRef.current = false;
      }
    } else {
      setShowMentions(false);
      isMentioningRef.current = false;
    }
  };

  const handleMentionSelect = (username: string) => {
    if (textareaRef.current && setFieldValueRef.current) {
      const textarea = textareaRef.current;
      const textBeforeAt = textarea.value.substring(0, atSymbolPosition);
      const textAfterCursor = textarea.value.substring(cursorPosition);
      
      const newText = textBeforeAt + '@' + username + ' ' + textAfterCursor;
      
      setFieldValueRef.current('content', newText);
      
      const newCursorPos = atSymbolPosition + username.length + 2;
      textarea.focus();
      textarea.setSelectionRange(newCursorPos, newCursorPos);
      
      setShowMentions(false);
      setMentionQuery('');
      isMentioningRef.current = false;
    }
  };

  const handleFormSubmit = async (values: { content: string }, { resetForm }: { resetForm: () => void }) => {
    try {
      await createComment(ticketId, values.content, selectedFiles, parentCommentId);
      resetForm();
      setSelectedFiles([]);
      setShowMentions(false);
      setMentionQuery('');
      isMentioningRef.current = false;
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
    <Segment clearing style={{ position: 'relative' }}>
      <Header content={isReply ? 'Add Reply' : 'Add Comment'} sub color='teal' />
      <Formik
        initialValues={{ content: '' }}
        validationSchema={validationSchema}
        onSubmit={handleFormSubmit}
      >
        {({ isSubmitting, isValid, setFieldValue, setTouched }) => {
          if (setFieldValue && setFieldValueRef.current !== setFieldValue) {
            setFieldValueRef.current = setFieldValue;
          }
          
          useEffect(() => {
            const handleOpenReply = () => {
              if (!isReply) {
                setTouched({ content: false });
              }
            };
            window.addEventListener('openReplyForm', handleOpenReply);
            return () => window.removeEventListener('openReplyForm', handleOpenReply);
          }, [setTouched]);
          
          return (
            <Form className='ui form'>
              <div style={{ position: 'relative' }}>
                <MyTextArea
                  name='content'
                  placeholder='Write your comment here... Type @ to mention users'
                  rows={3}
                  label=''
                  inputRef={textareaRef}
                  onChange={handleTextareaChange}
                />
                {showMentions && (
                  <MentionSuggestions
                    query={mentionQuery}
                    onSelect={handleMentionSelect}
                    onClose={() => {
                      setShowMentions(false);
                      isMentioningRef.current = false;
                    }}
                    position={mentionPosition}
                    projectId={ticketStore.selectedTicket?.projectId}
                  />
                )}
              </div>
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
          );
        }}
      </Formik>
    </Segment>
  );
});
