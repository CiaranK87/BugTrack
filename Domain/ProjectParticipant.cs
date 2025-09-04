namespace Domain
{
    public class ProjectParticipant
    {
        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsOwner { get; set; }
        public string Role { get; set; }
    }
}