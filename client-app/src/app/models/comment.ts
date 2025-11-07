export interface Comment {
  id: string;
  content: string;
  createdAt: Date;
  updatedAt?: Date;
  ticketId: string;
  authorId: string;
  authorUsername: string;
  authorDisplayName: string;
  attachments: CommentAttachment[];
}

export interface CommentAttachment {
  id: string;
  fileName: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  uploadedAt: Date;
  downloadUrl: string;
}

export interface CreateCommentForm {
  content: string;
  ticketId: string;
  attachments: File[];
}