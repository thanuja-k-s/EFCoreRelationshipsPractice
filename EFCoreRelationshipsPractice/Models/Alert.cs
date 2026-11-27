namespace EFCorePractice.Models
{
    public class Alert
    {
        public long Id { get; set; }

        public string SourceIp { get; set; } = "";

        public string DestinationIp { get; set; } = "";

        public string AlertType { get; set; } = "";

        public string Severity { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}