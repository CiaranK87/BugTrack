public class TicketDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Submitter { get; set; }
    public string Assigned { get; set; }
    public string Priority { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime Updated { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; }
}