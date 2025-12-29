using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using aws_secrets_migrator.Models;
using System.Text.Json;

namespace AwsSecretsMigrator.Services
{
    public class AwsSecretsService
    {
        private readonly IAmazonSecretsManager _client;

        public AwsSecretsService(IAmazonSecretsManager client)
        {
            _client = client;
        }

        public async Task SaveAsync(string secretName, List<SecretItem> items)
        {
            var json = JsonSerializer.Serialize(
                items.ToDictionary(x => x.Key, x => x.Value));

            try
            {
                await _client.PutSecretValueAsync(new PutSecretValueRequest
                {
                    SecretId = secretName,
                    SecretString = json
                });
            }
            catch (ResourceNotFoundException)
            {
                await _client.CreateSecretAsync(new CreateSecretRequest
                {
                    Name = secretName,
                    SecretString = json
                });
            }
        }
    }
}
