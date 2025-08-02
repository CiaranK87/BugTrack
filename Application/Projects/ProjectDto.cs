using Application.Profiles;

namespace Application.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }        

        public string ProjectTitle { get; set; }

        public string ProjectOwner { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public string OwnerUsername { get; set; }

        public bool IsCancelled { get; set; }

        public int TicketCount { get; set; }

        public ICollection<Profile> Participants { get; set; }
    }
}