using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PeanutButter.Utils;
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
    private readonly AppSettings _appSettings;

    public EmailService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    public async Task Send(EmailPayload emailPayload)
    {
        var apiKey = _appSettings.SendGridAccessKey;
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("mail@mergedigital.io", "Postbucket");
        var to = new EmailAddress(emailPayload.Recipient, emailPayload.Title);

        var template = await GetTemplate(emailPayload);
        
        var msg = MailHelper.CreateSingleEmail(from, to, emailPayload.Subject, null, template);
        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException("Something went wrong while trying to send the email to the email provider");
        }
    }

    private async Task<string> GetTemplate(EmailPayload payload)
    {
        // TODO cache this data...
        var blobServiceClient = new BlobServiceClient(_appSettings.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("templates");
        var blobClient = containerClient.GetBlobClient("primary-template.html");
        var file = await blobClient.DownloadContentAsync();

        if (!file.HasValue)
        {
            throw new BadHttpRequestException("Failed to retrieve the HTML template");
        }
        
        var templateBinary = file.Value.Content;
        var templateString = Encoding.UTF8.GetString(templateBinary);
        
        var htmlPageDocument = new HtmlDocument();
        htmlPageDocument.LoadHtml(templateString);
        
        var primaryNode = htmlPageDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'body-loop')]");
        var primaryNodeCopy = primaryNode.OuterHtml;
        var parentNode = primaryNode.ParentNode;
        
        if (primaryNodeCopy is null)
        {
            throw new BadHttpRequestException("Could not find the starting node in the HTML template");
        }
        
        parentNode.ChildNodes.Clear();

        foreach (var subNode in payload.Fields.Select(item => primaryNodeCopy
                     .Replace("{{message-title}}", item.Key.ToPascalCase())
                     .Replace("{{message-body}}", item.Value)))
        {
            parentNode.ChildNodes.Append(HtmlNode.CreateNode(subNode));
        }

        payload.Fields.TryGetValue("email", out var replyTo);

        return htmlPageDocument.DocumentNode.InnerHtml
            .Replace("{{website}}", payload.Website)
            .Replace("{{date}}", new DateTime().ToString("g"))
            .Replace("{{replyto}}", replyTo);
    }
}