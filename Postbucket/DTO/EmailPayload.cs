using System.Collections.Generic;

namespace Postbucket.DTO;

public class EmailPayload
{
    public EmailPayload()
    {
        Fields = new Dictionary<string, string>();
    }
    
    public string? Subject { get; set; }
    public string? Recipient { get; set; }
    public string? Website { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, string> Fields { get; set; }
}