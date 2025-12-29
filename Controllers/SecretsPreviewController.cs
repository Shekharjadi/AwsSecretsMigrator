using aws_secrets_migrator.Models;
using AwsSecretsMigrator.Services;
using Microsoft.AspNetCore.Mvc;

public class SecretsPreviewController : Controller
{
    private readonly AwsSecretsService _aws;

    public SecretsPreviewController(AwsSecretsService aws)
    {
        _aws = aws;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string secretName)
    {
        var model = new SecretsPreviewViewModel();

        // 🔹 Load dropdown secrets
        model.AvailableSecrets = await _aws.GetAllSecretNamesAsync();
        model.SecretName = secretName ?? string.Empty;

        if (string.IsNullOrWhiteSpace(secretName))
            return View(model);

        try
        {
            model.Secrets = await _aws.GetSecretsAsync(secretName);

            // ✅ SUCCESS TOAST
            TempData["ToastMessage"] = "Secrets loaded successfully!";
            TempData["ToastType"] = "success";
        }
        catch (Exception)
        {
            // ❌ ERROR TOAST
            TempData["ToastMessage"] = $"Secrets not found for: {secretName}";
            TempData["ToastType"] = "error";

            model.ErrorMessage = "Secrets are not found or inaccessible.";
        }

        return View(model);
    }
}
