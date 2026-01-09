
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace aws_secrets_migrator.Models
{
    public static class SecretsManager
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();

        public static async Task<T> LoadSecretAsync<T>(string secretName, string region = "eu-north-1", bool forceRefresh = false)
        {
            if (!forceRefresh && _cache.TryGetValue(secretName, out var cached) && cached is T cachedSecret)
            {
                return cachedSecret;
            }

            try
            {
                var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(region));

                var request = new GetSecretValueRequest
                {
                    SecretId = secretName
                };

                var response = await client.GetSecretValueAsync(request);

                if (string.IsNullOrEmpty(response.SecretString))
                    throw new Exception("SecretString is empty.");

                var secretObject = JsonSerializer.Deserialize<T>(response.SecretString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                _cache[secretName] = secretObject;

                return secretObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading secret '{secretName}': {ex.Message}");
                throw;
            }
        }

        public static void ClearCache(string secretName) => _cache.TryRemove(secretName, out _);

        public static void ClearAllCache() => _cache.Clear();
    }
}
