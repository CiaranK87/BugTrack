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
    <div className="comment-list-container">
      {commentStore.comments.length === 0 ? (
        <div className="no-comments-placeholder">
          <Icon name='comment outline' size='large' style={{ marginBottom: '10px', display: 'block' }} />
          No comments yet. Be the first to share your thoughts.
        </div>
      ) : (
        <div className="comment-list-wrapper">
          {sortedComments.filter(comment => !comment.parentCommentId).map((comment) => (
            <div key={comment.id} className="comment-item">
              <div className="comment-main-wrapper">
                <Image
                  src='/assets/user.png'
                  avatar
                  size='mini'
                  className="comment-avatar-img"
                />
                <div className="comment-body">
                  <div className="comment-header-row">
                    <div className="comment-author-info">
                      <span className="comment-author">
                        {comment.authorDisplayName}
                      </span>
                      <span className="comment-username">
                        @{comment.authorUsername}
                      </span>
                    </div>
                    <div className="comment-metadata">
                      <i className="clock icon"></i>
                      <span>
                        {format(new Date(comment.createdAt), 'PPp')}
                      </span>
                    </div>
                  </div>

                  <div className="comment-content">
                    {typeof comment.content === 'string' ? comment.content : JSON.stringify(comment.content)}
                  </div>

                  {comment.attachments && comment.attachments.length > 0 && (
                    <div className="comment-attachments">
                      <div className="attachment-header">
                        <Icon name='paperclip' style={{ marginRight: '6px' }} />
                        Attachments
                      </div>
                      <div className="attachments-grid">
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
                    <div className="comment-edited">
                      <Icon name='edit' />
                      Edited {format(new Date(comment.updatedAt), 'PPp')}
                    </div>
                  )}
                </div>
              </div>

              {/* Display replies */}
              {comment.replies && comment.replies.length > 0 && (
                <div className="comment-thread-replies">
                  {[...comment.replies].sort((a, b) =>
                    new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
                  ).map((reply) => (
                    <div key={reply.id} className="reply-item">
                      <div className="reply-main-wrapper">
                        <Image
                          src='/assets/user.png'
                          avatar
                          size='mini'
                          className="reply-avatar-img"
                        />
                        <div className="reply-body">
                          <div className="reply-header-row">
                            <div className="reply-author-info">
                              <span className="reply-author">
                                {reply.authorDisplayName}
                              </span>
                              <span className="reply-username">
                                @{reply.authorUsername}
                              </span>
                            </div>
                            <div className="reply-metadata">
                              <i className="clock icon"></i>
                              <span>
                                {format(new Date(reply.createdAt), 'PPp')}
                              </span>
                            </div>
                          </div>

                          <div className="reply-content">
                            {typeof reply.content === 'string' ? reply.content : JSON.stringify(reply.content)}
                          </div>

                          {reply.updatedAt && (
                            <div className="reply-edited">
                              <Icon name='edit' />
                              Edited {format(new Date(reply.updatedAt), 'PPp')}
                            </div>
                          )}

                          {/* Delete button for replies */}
                          {canDeleteComment(reply) && (
                            <div className="reply-actions-row">
                              <Button
                                icon='trash alternate'
                                basic
                                size='mini'
                                title='Delete reply'
                                className="delete-button"
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
              <div className="comment-footer-actions">
                <Button
                  size='mini'
                  basic
                  content='Reply'
                  icon='reply'
                  onClick={() => handleReply(comment.id)}
                  className="reply-btn-action"
                />

                {canDeleteComment(comment) && (
                  <div className="comment-delete-action">
                    <Button
                      icon='trash alternate'
                      basic
                      size='mini'
                      title='Delete comment'
                      className="delete-button"
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
                <div className="comment-reply-form">
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