namespace Domain
{
    public class Project
    {
        public Guid Id { get; set; }        

        public string ProjectTitle { get; set; }

        public string ProjectOwner { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public bool IsCancelled { get; set; }

        public ICollection<ProjectParticipant> Participants { get; set; } = new List<ProjectParticipant>();  
        public ICollection<Ticket> Tickets { get; set; }  
    }
}