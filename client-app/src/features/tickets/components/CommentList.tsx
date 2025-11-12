import React, { useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../../app/stores/store';
import { Icon, Button, Image, Confirm } from 'semantic-ui-react';
import { format } from 'date-fns';
import FileAttachment from './FileAttachment';
import CommentForm from './CommentForm';

interface Props {
  ticketId: string;
}

const CommentList: React.FC<Props> = observer(({ ticketId }) => {
  const { commentStore, userStore } = useStore();

  useEffect(() => {
    if (ticketId) {
      commentStore.setCurrentTicketId(ticketId);
      commentStore.loadComments(ticketId);
    }
  }, [ticketId]);

  useEffect(() => {
    const handleCloseReplyForm = (event: CustomEvent) => {
      if (event.detail.commentId) {
        setReplyingTo(null);
      }
    };

    window.addEventListener('closeReplyForm', handleCloseReplyForm as EventListener);
    
    return () => {
      window.removeEventListener('closeReplyForm', handleCloseReplyForm as EventListener);
    };
  }, []);

  const sortedComments = [...commentStore.comments].sort((a, b) => {
    return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
  });

  const [deleteConfirmOpen, setDeleteConfirmOpen] = React.useState<string | null>(null);
  const [replyingTo, setReplyingTo] = React.useState<string | null>(null);

  const handleDeleteComment = (commentId: string) => {
    commentStore.deleteComment(ticketId, commentId);
    setDeleteConfirmOpen(null);
  };

  const showDeleteConfirm = (commentId: string) => {
    setDeleteConfirmOpen(commentId);
  };

  const cancelDelete = () => {
    setDeleteConfirmOpen(null);
  };

  const handleReply = (commentId: string) => {
    setReplyingTo(replyingTo === commentId ? null : commentId);
  };

  const canDeleteComment = (comment: any) => {
    if (comment.authorUsername) {
      return userStore.isCurrentUser(comment.authorUsername);
    }
    return false;
  };

  return (
    <div style={{ marginTop: '20px' }}>
      {commentStore.comments.length === 0 ? (
        <div style={{
          textAlign: 'center',
          padding: '40px 20px',
          color: '#888',
          fontStyle: 'italic',
          backgroundColor: '#f9f9f9',
          borderRadius: '8px',
          border: '1px dashed #ddd'
        }}>
          <Icon name='comment outline' size='large' style={{ marginBottom: '10px', display: 'block' }} />
          No comments yet. Be the first to share your thoughts.
        </div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          {sortedComments.filter(comment => !comment.parentCommentId).map((comment) => (
            <div key={comment.id} style={{
              backgroundColor: '#fff',
              border: '1px solid #e8e8e8',
              borderRadius: '8px',
              padding: '16px',
              boxShadow: '0 1px 3px rgba(0,0,0,0.05)',
              transition: 'all 0.2s ease',
              minHeight: '120px'
            }}>
              <div style={{ display: 'flex', alignItems: 'flex-start' }}>
                <Image
                  src='/assets/user.png'
                  avatar
                  size='mini'
                  style={{ marginRight: '12px' }}
                />
                <div style={{ flex: 1 }}>
                  <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    marginBottom: '8px'
                  }}>
                    <div>
                      <span style={{
                        fontWeight: '600',
                        color: '#333',
                        fontSize: '0.95rem'
                      }}>
                        {comment.authorDisplayName}
                      </span>
                      <span style={{
                        color: '#888',
                        marginLeft: '8px',
                        fontSize: '0.85rem'
                      }}>
                        @{comment.authorUsername}
                      </span>
                    </div>
                    <div style={{
                      fontSize: '0.8rem',
                      color: '#888',
                      display: 'flex',
                      alignItems: 'center',
                      gap: '6px'
                    }}>
                      <i className="clock icon" style={{
                        margin: 0,
                        padding: 0,
                        fontSize: '0.9em',
                        lineHeight: '1em',
                        display: 'inline-block',
                        verticalAlign: 'middle'
                      }}></i>
                      <span style={{ lineHeight: '1em', display: 'inline-block' }}>
                        {format(new Date(comment.createdAt), 'PPp')}
                      </span>
                    </div>
                  </div>
                  
                  <div style={{
                    fontSize: '0.95rem',
                    lineHeight: '1.5',
                    color: '#444',
                    whiteSpace: 'pre-wrap',
                    wordBreak: 'break-word',
                    marginBottom: '12px',
                    minHeight: '40px',
                    padding: '8px',
                    borderRadius: '4px',
                    border: '1px solid rgba(0, 0, 0, 0.1)',
                    backgroundColor: '#fafbfc'
                  }}>
                    {typeof comment.content === 'string' ? comment.content : JSON.stringify(comment.content)}
                  </div>
                  
                  {comment.attachments && comment.attachments.length > 0 && (
                    <div style={{
                      backgroundColor: '#f8f9fa',
                      borderRadius: '6px',
                      padding: '10px',
                      marginBottom: '12px'
                    }}>
                      <div style={{
                        fontSize: '0.9rem',
                        color: '#666',
                        marginBottom: '6px',
                        display: 'flex',
                        alignItems: 'center'
                      }}>
                        <Icon name='paperclip' style={{ marginRight: '6px' }} />
                        Attachments
                      </div>
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                        {comment.attachments.map((attachment) => (
                          <FileAttachment
                            key={attachment.id}
                            attachment={attachment}
                            ticketId={ticketId}
                            commentId={comment.id}
                            onDownload={(attachmentId) => commentStore.downloadAttachment(ticketId, comment.id, attachmentId, attachment.originalFileName || attachment.fileName)}
                            onDelete={(attachmentId) => commentStore.deleteAttachment(ticketId, comment.id, attachmentId)}
                            showDeleteButton={canDeleteComment(comment)}
                          />
                        ))}
                      </div>
                    </div>
                  )}
                  
                  {comment.updatedAt && (
                    <div style={{
                      fontSize: '0.8rem',
                      color: '#aaa',
                      fontStyle: 'italic',
                      display: 'flex',
                      alignItems: 'center'
                    }}>
                      <Icon name='edit' style={{
                        marginRight: '4px',
                        fontSize: '0.9em',
                        verticalAlign: 'middle'
                      }} />
                      Edited {format(new Date(comment.updatedAt), 'PPp')}
                    </div>
                  )}
                </div>
              </div>
              
              {/* Display replies */}
              {comment.replies && comment.replies.length > 0 && (
                <div style={{ marginTop: '12px', marginLeft: '20px', paddingLeft: '16px', borderLeft: '2px solid #e8e8e8' }}>
                  {comment.replies.map((reply) => (
                    <div key={reply.id} style={{
                      backgroundColor: '#fff',
                      borderRadius: '8px',
                      padding: '12px',
                      marginBottom: '8px',
                      boxShadow: '0 1px 2px rgba(0,0,0,0.05)'
                    }}>
                      <div style={{ display: 'flex', alignItems: 'flex-start' }}>
                        <Image
                          src='/assets/user.png'
                          avatar
                          size='mini'
                          style={{ marginRight: '10px' }}
                        />
                        <div style={{ flex: 1 }}>
                          <div style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            marginBottom: '6px'
                          }}>
                            <div>
                              <span style={{
                                fontWeight: '600',
                                color: '#333',
                                fontSize: '0.9rem'
                              }}>
                                {reply.authorDisplayName}
                              </span>
                              <span style={{
                                color: '#888',
                                marginLeft: '6px',
                                fontSize: '0.8rem'
                              }}>
                                @{reply.authorUsername}
                              </span>
                            </div>
                            <div style={{
                              fontSize: '0.75rem',
                              color: '#888',
                              display: 'flex',
                              alignItems: 'center',
                              gap: '4px'
                            }}>
                              <i className="clock icon" style={{
                                margin: 0,
                                padding: 0,
                                fontSize: '0.9em',
                                lineHeight: '1em',
                                display: 'inline-block',
                                verticalAlign: 'middle'
                              }}></i>
                              <span style={{ lineHeight: '1em', display: 'inline-block' }}>
                                {format(new Date(reply.createdAt), 'PPp')}
                              </span>
                            </div>
                          </div>
                          
                          <div style={{
                            fontSize: '0.9rem',
                            lineHeight: '1.4',
                            color: '#444',
                            whiteSpace: 'pre-wrap',
                            wordBreak: 'break-word',
                            marginBottom: '8px',
                            padding: '6px',
                            borderRadius: '4px',
                            border: '1px solid rgba(0, 0, 0, 0.05)',
                            backgroundColor: '#fafbfc'
                          }}>
                            {typeof reply.content === 'string' ? reply.content : JSON.stringify(reply.content)}
                          </div>
                          
                          {reply.updatedAt && (
                            <div style={{
                              fontSize: '0.75rem',
                              color: '#aaa',
                              fontStyle: 'italic',
                              display: 'flex',
                              alignItems: 'center'
                            }}>
                              <Icon name='edit' style={{
                                marginRight: '3px',
                                fontSize: '0.9em',
                                verticalAlign: 'middle'
                              }} />
                              Edited {format(new Date(reply.updatedAt), 'PPp')}
                            </div>
                          )}
                          
                          {/* Delete button for replies */}
                          {canDeleteComment(reply) && (
                            <div style={{
                              display: 'flex',
                              justifyContent: 'flex-end',
                              marginTop: '6px'
                            }}>
                              <Button
                                icon='trash alternate'
                                basic
                                size='mini'
                                title='Delete reply'
                                style={{
                                  background: 'transparent',
                                  color: '#888',
                                  padding: '4px'
                                }}
                                onClick={() => showDeleteConfirm(reply.id)}
                              />
                              <Confirm
                                open={deleteConfirmOpen === reply.id}
                                content='Are you sure you want to delete this reply?'
                                onCancel={cancelDelete}
                                onConfirm={() => handleDeleteComment(reply.id)}
                                cancelButton='Cancel'
                                confirmButton='Delete'
                                size='mini'
                              />
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
              
              {/* Reply button and delete button */}
              <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                marginTop: '8px'
              }}>
                <Button
                  size='mini'
                  basic
                  content='Reply'
                  icon='reply'
                  onClick={() => handleReply(comment.id)}
                  style={{ fontSize: '0.8em' }}
                />
                
                {canDeleteComment(comment) && (
                  <div>
                    <Button
                      icon='trash alternate'
                      basic
                      size='mini'
                      title='Delete comment'
                      style={{
                        background: 'transparent',
                        color: '#888',
                        padding: '4px'
                      }}
                      onClick={() => showDeleteConfirm(comment.id)}
                    />
                    <Confirm
                      open={deleteConfirmOpen === comment.id}
                      content='Are you sure you want to delete this comment?'
                      onCancel={cancelDelete}
                      onConfirm={() => handleDeleteComment(comment.id)}
                      cancelButton='Cancel'
                      confirmButton='Delete'
                      size='mini'
                    />
                  </div>
                )}
              </div>
              
              {/* Reply form */}
              {replyingTo === comment.id && (
                <div style={{ marginTop: '12px', marginLeft: '20px' }}>
                  <CommentForm ticketId={ticketId} parentCommentId={comment.id} isReply={true} />
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
});

export default CommentList;