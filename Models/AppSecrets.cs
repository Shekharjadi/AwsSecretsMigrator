namespace aws_secrets_migrator.Models
{
    public class AppSecrets
    {
        public string OnBaseURL { get; set; }
        public string GC_OnBaseURL { get; set; }
        public bool ShowXMLs { get; set; }
        public bool ShowSimulator { get; set; }
        public bool LogDocumentsInDB { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public string AwsSecreteConnection { get; set; }
    }
}