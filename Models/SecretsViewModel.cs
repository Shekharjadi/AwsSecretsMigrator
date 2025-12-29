using System.ComponentModel.DataAnnotations;

namespace aws_secrets_migrator.Models;

public class SecretsViewModel
{
    [Required]
    [Display(Name = "AWS Secret Name")]
    public string SecretName { get; set; }
    public string FileName { get; set; }
    public string Environment { get; set; }
    public List<SecretItem> Secrets { get; set; } = new List<SecretItem>();
}
