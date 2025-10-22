using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AdminRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Username { get; set; }
        
        public string Role { get; set; }
        
        public string JobTitle { get; set; }
    }
}