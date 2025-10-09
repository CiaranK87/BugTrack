using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Bio { get; set; }
        public string GlobalRole { get; set; } = "User";
        public ICollection<ProjectParticipant> ProjectParticipants { get; set; }
    }
}