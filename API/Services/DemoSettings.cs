namespace API.Services
{
    public class DemoSettings
    {
        public bool EnableNightlyReseed { get; set; } = false;
        public string ReseedTimeUtc { get; set; } = "02:00";
    }
}
