namespace JobApplicationLibrary.Services;

public interface IIdentityValidator
{
    bool IsValid(string identityNumber);
    
    ICountryDataProvider CountryDataProvider { get; }
    
    public ValidationMode ValidationMode { get; set; }
}

public enum ValidationMode
{
    Quick,
    Detailed
}

public interface ICountryData
{
    string Country { get; }
}

public interface ICountryDataProvider
{
    ICountryData CountryData { get; }
}