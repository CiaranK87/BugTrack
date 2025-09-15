namespace Application.Projects
{
    public class EditProjectDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
    }
}
