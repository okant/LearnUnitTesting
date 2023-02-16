namespace JobApplicationLibrary.Services;

public class IdentityValidator : IIdentityValidator
{
    public bool IsValid(string identityNumber)
    {
        return true;
    }

    public ICountryDataProvider CountryDataProvider { get; }
    public ValidationMode ValidationMode { get; set; }
}