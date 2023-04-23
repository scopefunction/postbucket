using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Postbucket.DTO;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace Postbucket.BLL;

public interface IEmailService
{
    Task Send(EmailPayload emailPayload);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly AppSettings _appSettings;

    public EmailService(IConfiguration configuration,
        AppSettings appSettings)
    {
        _configuration = configuration;
        _appSettings = appSettings;
    }
    
    public async Task Send(EmailPayload emailPayload)
    {
        var apiKey = Environment.GetEnvironmentVariable("POSTBUCKET_SENDGRID_KEY");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("mail@mergedigital.io", "Merge Digital");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress("mail@mergedigital.io", "Merge Digital");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }

    private async Task<string> GetTemplate()
    {
        var blobServiceClient = new BlobServiceClient(_appSettings.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("templates");
        var blobClient = containerClient.GetBlobClient("primary-template.html");
        var file = await blobClient.DownloadContentAsync();
        var templateBinary = file.Value.Content;
        return "";
    }
}