using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Graph;
using Microsoft.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MP133
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly SecretClient _secretClient;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;
        private string _senderEmail;
        private bool _secretsInitialized = false;

        public EmailService(IConfiguration config, SecretClient secretClient, ILogger<EmailService> logger, HttpClient httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _secretClient = secretClient ?? throw new ArgumentNullException(nameof(secretClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Öffentlich machen, um sie explizit aufzurufen
        public async Task InitializeSecretsAsync()
        {
            if (_secretsInitialized) return;

            try
            {
                // Abruf des "SenderEmail"-Secrets
                _senderEmail = await GetSecretAsync("SenderEmail").ConfigureAwait(false);
                _secretsInitialized = true;

                _logger.LogInformation("Secrets erfolgreich initialisiert.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Initialisieren der Secrets: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailAsync(string fromEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_senderEmail))
            {
                // Sicherstellen, dass die Secrets initialisiert sind
                await InitializeSecretsAsync().ConfigureAwait(false);
            }

            try
            {
                string accessToken = await GetAccessTokenAsync().ConfigureAwait(false);

                // E-Mail-Nachricht erstellen
                var message = new
                {
                    message = new
                    {
                        from = new { emailAddress = new { address = fromEmail } },
                        subject = subject,
                        body = new
                        {
                            contentType = "HTML",
                            content = body
                        },
                        toRecipients = new[] { new { emailAddress = new { address = _senderEmail } } }
                    },
                    saveToSentItems = "true"
                };

                string jsonContent = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/users/{_senderEmail}/sendMail")
                {
                    Content = content
                };
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Senden der Anfrage
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"E-Mail erfolgreich gesendet. Empfänger: {_senderEmail}, Betreff: {subject}");
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError($"Fehler beim Senden der E-Mail: {response.StatusCode} - {response.ReasonPhrase}. Antwort: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Senden der E-Mail: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var tokenRequestContext = new Azure.Core.TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
                var accessToken = await credential.GetTokenAsync(tokenRequestContext);

                return accessToken.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Abrufen des Access Tokens: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName).ConfigureAwait(false);
                return secret.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fehler beim Abrufen des Secrets '{secretName}': {ex.Message}");
                throw;
            }
        }
    }
}