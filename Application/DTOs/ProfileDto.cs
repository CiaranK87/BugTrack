namespace Application.DTOs
{
    public class ProfileDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
    }
}