using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Postbucket.Models;

namespace Postbucket.Services;

public interface IDocumentService
{
    Task SaveSubmission(Dictionary<string, string> fields);
    Task<ValidatedFormResponse> ValidateFormId(string? formId);
}

public class GenericEntity : ITableEntity
{
    public GenericEntity()
    {
        Timestamp = DateTimeOffset.Now;
        ETag = new ETag();
        var compositeKey = Guid.NewGuid();
        PartitionKey = compositeKey.ToString();
        RowKey = compositeKey.ToString();
    }
        
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public string? Data { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

public class DocumentService : IDocumentService
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<DocumentService> _logger;
    private readonly AppSettings _appSettings;

    public DocumentService(
        CosmosClient cosmosClient, 
        ILogger<DocumentService> logger, 
        AppSettings appSettings)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
        _appSettings = appSettings;
    }

    public async Task SaveSubmission(Dictionary<string, string> fields)
    {
        try
        {
            var tableClient = GetOrCreateTableClient("submissions");
            await tableClient.UpsertEntityAsync(new GenericEntity
            {
                Data = JsonSerializer.Serialize(fields)
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred within DocumentService when trying to save the submission");
        }
    }

    private TableClient GetOrCreateTableClient(string table)
    {
        var account = new TableServiceClient(_appSettings.BlobStorageConnectionString);
        var storageClient = account.GetTableClient(table);
        return storageClient;
    }

    public async Task<ValidatedFormResponse> ValidateFormId(string? formId)
    {
        try
        {
            var tableClient = GetOrCreateTableClient("forms");
            var entity = tableClient.GetEntity<GenericEntity>(formId, formId);

            if (!entity.HasValue)
            {
                return new ValidatedFormResponse
                {
                    IsValid = false
                };
            }

            var deserializedContent = JsonSerializer.Deserialize<FormContext>(entity.Value.Data ?? string.Empty,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return new ValidatedFormResponse
            {
                IsValid = true,
                Form = deserializedContent
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