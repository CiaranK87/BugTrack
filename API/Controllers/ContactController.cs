using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using API.Extensions;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IConfiguration config, ILogger<ContactController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost("request-access")]
        public async Task<IActionResult> RequestAccess([FromBody] AccessRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) || 
                    string.IsNullOrEmpty(request.Email) || 
                    string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest("All fields are required");
                }

                // Validate email format
                try
                {
                    var mailAddress = new MailAddress(request.Email);
                }
                catch
                {
                    return BadRequest("Invalid email format");
                }

                // Log the request details
                _logger.LogInformation($"Access request received from {request.Email} ({request.Name}) with message: {request.Message}");
                
                // Send email notification (only if SMTP is configured)
                try
                {
                    await SendAccessRequestEmail(request);
                    _logger.LogInformation($"Access request received from {request.Email} ({request.Name}) - Email sent successfully");
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, $"Failed to send email for access request from {request.Email} ({request.Name})");
                    
                    // Log to file as backup
                    await LogRequestToFile(request);
                    
                    _logger.LogInformation($"Access request received from {request.Email} ({request.Name}) - Email failed but request logged to file");
                }

                return Ok(new { message = "Access request sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing access request");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task SendAccessRequestEmail(AccessRequestDto request)
        {
            var adminEmail = _config.GetEnvironmentVariable("CONTACT_ADMIN_EMAIL") ?? "ciarank500@gmail.com";
            var fromEmail = _config.GetEnvironmentVariable("CONTACT_FROM_EMAIL") ?? "onboarding@resend.dev";
            var resendApiKey = _config.GetEnvironmentVariable("RESEND_API_KEY");

            // Skip email sending if Resend API key is not configured
            if (string.IsNullOrEmpty(resendApiKey))
            {
                _logger.LogWarning("Resend API key not configured. Skipping email send.");
                return;
            }

            try
            {
                var htmlContent = $@"
<h3>New access request for BugTrack</h3>
<p><strong>Name:</strong> {request.Name}</p>
<p><strong>Email:</strong> {request.Email}</p>
<p><strong>Message:</strong> {request.Message}</p>
<p><strong>Requested at:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
<p>Please review this request and respond accordingly.</p>";

                var plainTextContent = $@"
New access request for BugTrack:

Name: {request.Name}
Email: {request.Email}
Message: {request.Message}
Requested at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

Please review this request and respond accordingly.";

                var emailData = new
                {
                    from = fromEmail,
                    to = new[] { adminEmail },
                    subject = $"BugTrack Access Request from {request.Name}",
                    html = htmlContent,
                    text = plainTextContent
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {resendApiKey}");

                var response = await httpClient.PostAsync("https://api.resend.com/emails", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Email sent successfully to {AdminEmail} using Resend. Response: {Response}", adminEmail, responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Resend API returned status {StatusCode}: {Error}", response.StatusCode, errorContent);
                    throw new Exception($"Resend API error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend Error: {Message}", ex.Message);
                throw;
            }
        }

        private async Task LogRequestToFile(AccessRequestDto request)
        {
            try
            {
                var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logFile = Path.Combine(logDirectory, "access-requests.log");
                var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] Name: {request.Name}, Email: {request.Email}, Message: {request.Message}{Environment.NewLine}";
                
                await System.IO.File.AppendAllTextAsync(logFile, logEntry);
                _logger.LogInformation("Access request logged to file: {LogFile}", logFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log access request to file");
            }
        }
    }

    public class AccessRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}