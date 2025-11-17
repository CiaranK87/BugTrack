import { Icon, Label, Button, Modal, Image } from 'semantic-ui-react';
import { useState, useEffect } from 'react';
import { CommentAttachment } from '../../../app/models/comment';
import axios from 'axios';
import { logger } from '../../../app/utils/logger';

interface Props {
  attachment: CommentAttachment;
  ticketId: string;
  commentId: string;
  onDownload?: (attachmentId: string) => void;
  onDelete?: (attachmentId: string) => void;
  showDeleteButton?: boolean;
}

export default function FileAttachment({
  attachment,
  ticketId,
  commentId,
  onDownload,
  onDelete,
  showDeleteButton = false
}: Props) {
  const [viewModalOpen, setViewModalOpen] = useState(false);
  const handleDownload = async () => {
    if (onDownload) {
      onDownload(attachment.id);
    } else {
      logger.debug('Download clicked for attachment', attachment);
      
      try {
        let downloadUrl = attachment.downloadUrl;
        if (!downloadUrl) {
          downloadUrl = `/tickets/${ticketId}/comments/${commentId}/attachments/${attachment.id}/download`;
        } else {
          if (downloadUrl.startsWith('/api/')) {
            downloadUrl = downloadUrl.substring(4);
          }
        }
        
        const response = await axios.get(downloadUrl, {
          responseType: 'blob',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwt')}`
          }
        });
        
        const blob = new Blob([response.data], { type: attachment.contentType });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = attachment.originalFileName || attachment.fileName;
        document.body.appendChild(link);
        link.click();
        
        setTimeout(() => {
          document.body.removeChild(link);
          URL.revokeObjectURL(url);
        }, 100);
        
      } catch (error) {
        logger.error('Failed to download file', error);
        const link = document.createElement('a');
        let downloadUrl = attachment.downloadUrl || `http://localhost:5000/api/tickets/${ticketId}/comments/${commentId}/attachments/${attachment.id}/download`;
        link.href = downloadUrl;
        link.download = attachment.originalFileName || attachment.fileName;
        document.body.appendChild(link);
        link.click();
        setTimeout(() => {
          document.body.removeChild(link);
        }, 100);
      }
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

  const isImageFile = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();
    return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'].includes(extension || '');
  };

  const isPdfFile = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();
    return extension === 'pdf';
  };

  const isTextFile = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase();
    return ['txt', 'md', 'json', 'xml', 'csv'].includes(extension || '');
  };

  const renderFilePreview = () => {
    if (isImageFile(attachment.fileName)) {
      return <AuthenticatedImage attachment={attachment} ticketId={ticketId} commentId={commentId} />;
    }
    
    if (isPdfFile(attachment.fileName)) {
      return (
        <div style={{ width: '100%', height: '70vh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
          <Icon name="file pdf outline" size="huge" />
          <p>PDF preview not available</p>
          <p>Please download the file to view its contents</p>
        </div>
      );
    }
    
    if (isTextFile(attachment.fileName)) {
      return (
        <div style={{
          width: '100%',
          height: '70vh',
          overflow: 'auto',
          border: '1px solid #ccc',
          padding: '10px',
          backgroundColor: '#f9f9f9',
          fontFamily: 'monospace',
          fontSize: '14px',
          whiteSpace: 'pre-wrap'
        }}>
          <p>Text file preview not available. Please download to view the full content.</p>
        </div>
      );
    }
    
    return (
      <div style={{
        textAlign: 'center',
        padding: '50px',
        color: '#666'
      }}>
        <Icon name="file" size="huge" />
        <p>Preview not available for this file type</p>
        <p>Please download the file to view its contents</p>
      </div>
    );
  };

  return (
    <>
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
          onClick={() => setViewModalOpen(true)}
          style={{ marginRight: '5px', padding: '2px' }}
          title="View file"
        >
          <Icon name="eye" />
        </Button>
        <Button
          icon
          size="mini"
          compact
          onClick={handleDownload}
          style={{ marginRight: '5px', padding: '2px' }}
          title="Download file"
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
            title="Delete file"
          >
            <Icon name="delete" />
          </Button>
        )}
      </Label>

      <Modal
        open={viewModalOpen}
        onClose={() => setViewModalOpen(false)}
        size="large"
        closeIcon
      >
        <Modal.Header>
          <Icon name={getFileIcon(attachment.fileName)} />
          {attachment.fileName}
          <span style={{ fontSize: '0.8em', color: '#666', marginLeft: '10px' }}>
            ({formatFileSize(attachment.fileSize)})
          </span>
        </Modal.Header>
        <Modal.Content scrolling>
          {renderFilePreview()}
        </Modal.Content>
        <Modal.Actions>
          <Button onClick={() => setViewModalOpen(false)}>
            Close
          </Button>
          <Button primary onClick={handleDownload}>
            <Icon name="download" />
            Download
          </Button>
        </Modal.Actions>
      </Modal>
    </>
  );
}

interface AuthenticatedImageProps {
  attachment: CommentAttachment;
  ticketId: string;
  commentId: string;
}

function AuthenticatedImage({ attachment, ticketId, commentId }: AuthenticatedImageProps) {
  const [imageUrl, setImageUrl] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchImage = async () => {
      try {
        setLoading(true);
        setError('');
        
        let downloadUrl = attachment.downloadUrl;
        if (!downloadUrl) {
          downloadUrl = `/tickets/${ticketId}/comments/${commentId}/attachments/${attachment.id}/download`;
        } else {
          if (downloadUrl.startsWith('/api/')) {
            downloadUrl = downloadUrl.substring(4);
          }
        }
        
        const response = await axios.get(downloadUrl, {
          responseType: 'blob',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwt')}`
          }
        });
        
        const blob = new Blob([response.data], { type: attachment.contentType });
        const url = URL.createObjectURL(blob);
        setImageUrl(url);
        
        return () => URL.revokeObjectURL(url);
      } catch (err) {
        logger.error('Failed to fetch image', err);
        setError('Failed to load image');
      } finally {
        setLoading(false);
      }
    };

    fetchImage();
  }, [attachment, ticketId, commentId]);

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '50px' }}>
        <Icon name="spinner" loading size="large" />
        <p>Loading image...</p>
      </div>
    );
  }

  if (error || !imageUrl) {
    return (
      <div style={{ textAlign: 'center', padding: '50px', color: '#666' }}>
        <Icon name="image" size="huge" />
        <p>{error || 'Failed to load image'}</p>
        <p>Please try downloading the file instead</p>
      </div>
    );
  }

  return (
    <div style={{ textAlign: 'center' }}>
      <Image
        src={imageUrl}
        fluid
        style={{ maxHeight: '70vh', objectFit: 'contain' }}
        onError={() => {
          logger.error('Failed to display image from blob URL');
          setError('Failed to display image');
        }}
        onLoad={() => {
          logger.debug('Image loaded successfully from blob URL');
        }}
      />
    </div>
  );
}