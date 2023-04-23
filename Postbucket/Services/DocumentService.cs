using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Postbucket.Models;

namespace Postbucket.Services;

public interface IDocumentService
{
    Task SaveSubmission(Dictionary<string, string> fields);
    Task<ValidatedFormResponse> ValidateFormId(string? formId);
}

public class DocumentService : IDocumentService
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(CosmosClient cosmosClient, ILogger<DocumentService> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    public async Task SaveSubmission(Dictionary<string, string> fields)
    {
        try
        {
            var database = _cosmosClient.GetDatabase("Postbucket");
            var container = database.GetContainer("submissions");
            fields.TryAdd("id", Guid.NewGuid().ToString());
            await container.CreateItemAsync(fields);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred within DocumentService when trying to save the submission");
        }
    }

    public async Task<ValidatedFormResponse> ValidateFormId(string? formId)
    {
        try
        {
            var database = _cosmosClient.GetDatabase("Postbucket");
            var container = database.GetContainer("forms");
            
            return new ValidatedFormResponse
            {
                IsValid = true,
                Form = await container.ReadItemAsync<FormContext>(formId, new PartitionKey(formId))
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred within DocumentService");
            return new ValidatedFormResponse
            {
                IsValid = false
            };
        }
    }
}