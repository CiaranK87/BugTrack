namespace Domain
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public string Submitter { get; set; }
        public string Assigned { get; set; }
        public string Priority { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Updated { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}