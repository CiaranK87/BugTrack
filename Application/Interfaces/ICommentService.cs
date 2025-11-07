using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetCommentsAsync(Guid ticketId);
        Task<CommentDto> GetCommentAsync(Guid ticketId, Guid id);
        Task<CommentDto> CreateCommentAsync(Guid ticketId, CreateCommentDto commentDto, string userId);
        Task<CommentDto> UpdateCommentAsync(Guid ticketId, Guid id, UpdateCommentDto commentDto, string userId);
        Task<bool> DeleteCommentAsync(Guid ticketId, Guid id, string userId);
        Task<CommentAttachmentDto> AddAttachmentAsync(Guid ticketId, Guid commentId, IFormFile file, string userId);
        Task<(Stream FileStream, string FileName, string ContentType)> GetAttachmentAsync(Guid ticketId, Guid commentId, Guid attachmentId);
        Task<bool> DeleteAttachmentAsync(Guid ticketId, Guid commentId, Guid attachmentId, string userId);
    }
}