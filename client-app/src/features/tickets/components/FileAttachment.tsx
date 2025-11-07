import { Icon, Label, Button } from 'semantic-ui-react';
import { CommentAttachment } from '../../../app/models/comment';

interface Props {
  attachment: CommentAttachment;
  onDownload?: (attachmentId: string) => void;
  onDelete?: (attachmentId: string) => void;
  showDeleteButton?: boolean;
}

export default function FileAttachment({
  attachment,
  onDownload,
  onDelete,
  showDeleteButton = false
}: Props) {
  const handleDownload = () => {
    if (onDownload) {
      onDownload(attachment.id);
    } else {
      // Default download behavior - create a temporary link and click it
      const link = document.createElement('a');
      link.href = `http://localhost:5000/api/comments/attachments/${attachment.id}/download`;
      link.download = attachment.originalFileName || attachment.fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  };

  const handleDelete = () => {
    if (onDelete) {
      onDelete(attachment.id);
    }
  };

  const getFileIcon = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();
    
    switch (extension) {
      case 'pdf':
        return 'file pdf';
      case 'doc':
      case 'docx':
        return 'file word';
      case 'xls':
      case 'xlsx':
        return 'file excel';
      case 'ppt':
      case 'pptx':
        return 'file powerpoint';
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'bmp':
        return 'file image';
      case 'zip':
      case 'rar':
      case '7z':
        return 'file archive';
      case 'txt':
        return 'file text';
      default:
        return 'file';
    }
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes';
    
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  return (
    <Label style={{ margin: '2px', display: 'inline-flex', alignItems: 'center' }}>
      <Icon name={getFileIcon(attachment.fileName)} />
      <span style={{ marginRight: '5px' }}>{attachment.fileName}</span>
      <span style={{ fontSize: '0.8em', color: '#666', marginRight: '5px' }}>
        ({formatFileSize(attachment.fileSize)})
      </span>
      <Button
        icon
        size="mini"
        compact
        onClick={handleDownload}
        style={{ marginRight: '5px', padding: '2px' }}
      >
        <Icon name="download" />
      </Button>
      {showDeleteButton && (
        <Button
          icon
          size="mini"
          compact
          onClick={handleDelete}
          style={{ padding: '2px' }}
        >
          <Icon name="delete" />
        </Button>
      )}
    </Label>
  );
}