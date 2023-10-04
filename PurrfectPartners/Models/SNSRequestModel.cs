namespace PurrfectPartners.Models
{
    public class SNSRequestModel
    {

        public List<string> AWSKeys { get; set; } = new();

        public string Action { get; set; } = null!;

        public string? Email { get; set; }

        public string? Message { get; set; }

        public string? Subject { get; set; }

    }
}
