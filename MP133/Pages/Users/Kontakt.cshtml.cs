using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MP133.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace MP133.Pages.Users
{
    public class KontaktModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly IOptions<ReCaptchaSettings> _reCaptchaSettings;
        private readonly HttpClient _httpClient;
        private readonly SecretClient _secretClient;

        public KontaktModel(EmailService emailService, IOptions<ReCaptchaSettings> reCaptchaSettings, HttpClient httpClient, SecretClient secretClient)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _reCaptchaSettings = reCaptchaSettings ?? throw new ArgumentNullException(nameof(reCaptchaSettings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _secretClient = secretClient ?? throw new ArgumentNullException(nameof(secretClient));
        }

        [BindProperty]
        public ContactForm Form { get; set; }
        public string ReCaptchaSiteKey { get; set; }

        public async Task OnGetAsync()
        {
            Form = new ContactForm();
            ReCaptchaSiteKey = await GetSecretAsync("SiteKey");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Hole das Secret für reCAPTCHA vom Azure Key Vault
            var reCaptchaSecretKey = await GetSecretAsync("SecretKey");

            // Überprüfe reCAPTCHA Antwort
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            var isCaptchaValid = await VerifyCaptchaAsync(recaptchaResponse, reCaptchaSecretKey);

            if (!isCaptchaValid)
            {
                ModelState.AddModelError(string.Empty, "Die CAPTCHA-Verifizierung ist fehlgeschlagen. Bitte versuche es erneut.");
                return Page();
            }

            // E-Mail-Inhalt vorbereiten
            var subject = "Neue Kontaktanfrage";
            var body = $"Name: {Form.Name}\nE-Mail: {Form.Email}\nBetreff: {Form.Betreff}\nNachricht: {Form.Message}";

            try
            {
                // E-Mail senden
                await _emailService.SendEmailAsync(Form.Email, subject, body);

                // Erfolgsnachricht setzen
                TempData["Success"] = "Ihre Nachricht wurde erfolgreich gesendet!";

                Form = new ContactForm();
                return RedirectToPage("./Kontakt");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Fehler beim Senden der E-Mail: {ex.Message}");
                return Page();
            }
        }

        private async Task<bool> VerifyCaptchaAsync(string recaptchaResponse, string secretKey)
        {
            try
            {
                var verificationUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}";

                var response = await _httpClient.GetStringAsync(verificationUrl);
                var captchaVerification = JsonConvert.DeserializeObject<CaptchaVerificationResponse>(response);

                return captchaVerification.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der CAPTCHA-Verifizierung: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Fehler beim Abrufen des Secrets {secretName}: {ex.Message}", ex);
            }
        }

        public class CaptchaVerificationResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public string[] ErrorCodes { get; set; }
        }

        public class ContactForm
        {
            [Required(ErrorMessage = "Name ist erforderlich.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "E-Mail ist erforderlich.")]
            [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige E-Mail-Adresse ein.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Betreff ist erforderlich.")]
            public string Betreff { get; set; }

            [Required(ErrorMessage = "Nachricht ist erforderlich.")]
            public string Message { get; set; }
        }
    }
}

