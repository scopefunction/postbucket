using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Postbucket.Services;

public interface IDocumentService
{
    void SaveSubmission(object data);
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

    public void SaveSubmission(object data)
    {
        
    }

    public async Task<ValidatedFormResponse> ValidateFormId(string? formId)
    {
        var database = _cosmosClient.GetDatabase("postbucket-cosmos");
        var container = database.GetContainer("forms");

        try
        {
            return new ValidatedFormResponse
            {
                IsValid = true,
                Form = await container.ReadItemAsync<FormEntity>(formId, new PartitionKey(formId))
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