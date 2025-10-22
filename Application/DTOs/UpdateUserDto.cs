using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string DisplayName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        public string JobTitle { get; set; }
        
        public string Bio { get; set; }
    }
}