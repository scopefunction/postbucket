using Postbucket.Models;

namespace Postbucket.Services;

public class ValidatedFormResponse
{
    public FormContext? Form { get; set; }
    public bool IsValid { get; set; }
}