namespace aws_secrets_migrator.Models;
public class AppSettings
{
    public string OnBaseURL { get; set; }
    public string GC_OnBaseURL { get; set; }
    public bool ShowXMLs { get; set; }
    public bool ShowSimulator { get; set; }
    public bool LogDocumentsInDB { get; set; }
}
