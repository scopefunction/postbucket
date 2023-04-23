namespace Postbucket.Models;

public class FormContext
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Email { get; set; }
    public bool IsValid { get; set; }
    public string? Website { get; set; }
}