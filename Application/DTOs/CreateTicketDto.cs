namespace Application.DTOs
{
    public class CreateTicketDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Assigned { get; set; }
        public string Priority { get; set; }
        public string Severity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid ProjectId { get; set; }
    }
}
