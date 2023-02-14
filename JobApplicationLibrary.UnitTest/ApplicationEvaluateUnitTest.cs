using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary.UnitTest;

public class ApplicationEvaluateUnitTest
{
    /// <summary>
    /// Test Yapılacak En Küçük Parça Gibi Düşünelim (Application)
    /// Gönderilecek Parametrenin Durumu Gibi Düşünelim (WithUnderAge)
    /// Beklenen Sonuç Gibi Düşünelim (TransferredToAutoRejected)
    /// UnitOfWork_Condition_ExpectedResult
    /// </summary>
    [Test]
    public void Application_WithUnderAge_TransferredToAutoRejected()
    {
        // Arrange

        // Constructor'a IIdentityValidator inject edildikten sonra parametre almalı, bu test kapsamında inject edilen servise ihtiyaç olmadığı için null eklenebilir
        var evaluator = new ApplicationEvaluator(null);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 17 }
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        Assert.AreEqual(result, ApplicationResult.AutoRejected);
    }

    [Test]
    public void Application_WithNoTechStack_TransferredToAutoRejected()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 22 },
            TechStackList = new List<string> { "" }
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        Assert.AreEqual(result, ApplicationResult.AutoRejected);
    }

    [Test]
    public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 37 },
            TechStackList = new List<string> { "C#", "RabbitMQ", "Microservices", "Visual Studio" },
            YearsOfExperience = 16
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        Assert.AreEqual(result, ApplicationResult.AutoAccepted);
    }
    
    [Test]
    public void Application_WithInvalidIdentityNumber_TransferredToAutoAccepted()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 37 }
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        Assert.AreEqual(result, ApplicationResult.TransferredToHR);
    }
}