using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary;

public class ApplicationEvaluator
{
    private const int MinAge = 18;
    private const int AutoAcceptedYearOfExperience = 15;
    private readonly IIdentityValidator _identityValidator;
    private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservices", "Visual Studio" };

    public ApplicationEvaluator(IIdentityValidator identityValidator)
    {
        _identityValidator = identityValidator;
    }

    public ApplicationResult Evaluate(JobApplication form)
    {
        if (form.Applicant is null) throw new ArgumentNullException();
        if (form.Applicant.Age < MinAge) return ApplicationResult.AutoRejected;

        _identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

        if (_identityValidator.CountryDataProvider.CountryData.Country != "Turkey")
            return ApplicationResult.TransferredToCTO;

        var validIdentity = _identityValidator.IsValid(form.Applicant.IdentityNumber);

        if (!validIdentity) return ApplicationResult.TransferredToHR;

        var ratio = GetTechStackSimilarityRate(form.TechStackList);

        return ratio switch
        {
            < 25 => ApplicationResult.AutoRejected,
            > 75 when form.YearsOfExperience >= AutoAcceptedYearOfExperience => ApplicationResult.AutoAccepted,
            _ => ApplicationResult.AutoAccepted
        };
    }

    private int GetTechStackSimilarityRate(IEnumerable<string> applicantTechStack)
    {
        var matchedCount = applicantTechStack.Count(s => techStackList.Contains(s, StringComparer.OrdinalIgnoreCase));
        return (int)((double)matchedCount / techStackList.Count) * 100;
    }
}

public enum ApplicationResult
{
    AutoRejected,
    TransferredToHR,
    TransferredToLead,
    TransferredToCTO,
    AutoAccepted
}