namespace Postbucket.Services;

public class ValidatedFormResponse
{
    public FormEntity? Form { get; set; }
    public bool IsValid { get; set; }
}