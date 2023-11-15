namespace Domain
{
    public class Project
    {
        public Guid Id { get; set; }        

        public string Name { get; set; }

        public string ProjectOwner { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}