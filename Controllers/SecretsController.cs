using aws_secrets_migrator.Models;
using AwsSecretsMigrator.Services;
using Microsoft.AspNetCore.Mvc;


namespace AwsSecretsMigrator.Controllers
{
    public class SecretsController : Controller
    {
        private readonly AwsSecretsService _aws;

        public SecretsController(AwsSecretsService aws)
        {
            _aws = aws;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new SecretsViewModel());
        }
[HttpPost]
public IActionResult LoadFile(IFormFile secretsFile, string environment, string secretName)
{
    if (secretsFile == null || secretsFile.Length == 0)
    {
        ModelState.AddModelError("", "Please upload a valid TXT file");
        return View("Index", new SecretsViewModel());
    }

    var secrets = TxtSecretReader.ReadFromStream(secretsFile.OpenReadStream());

    return View("Index", new SecretsViewModel
    {
        Environment = environment,
        SecretName = secretName,
        Secrets = secrets
    });
}


        [HttpPost]
        public async Task<IActionResult> Migrate(SecretsViewModel model)
        {
            var secretName = $"{model.SecretName}/{model.FileName}";

            await _aws.SaveAsync(secretName, model.Secrets);

            ViewBag.Message = "Secrets migrated successfully to AWS Secrets Manager!";
            return View("Index", model);
        }
    }
}
