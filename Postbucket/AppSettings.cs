namespace Postbucket;

public class AppSettings
{
    public AppSettings()
    {
        BlobStorageConnectionString = string.Empty;
        CosmosConnectionString = string.Empty;
        SendGridAccessKey = string.Empty;
        FromEmail = string.Empty;
    }
    
    public string BlobStorageConnectionString { get; set; }
    public string CosmosConnectionString { get; set; }
    public string SendGridAccessKey { get; set; }
    public string FromEmail { get; set; }
}