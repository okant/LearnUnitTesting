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

        //Assert.AreEqual(result, ApplicationResult.AutoRejected);
        //FluentAssertion version
        result.Should().Be(ApplicationResult.AutoRejected);
    }

    [Test]
    public void Application_WithNoTechStack_TransferredToAutoRejected()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

        // IsValid metodu Exception fırlatmış gibi de mocklanmasını bu şekilde sağlayabiliyoruz
        // mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Throws<Exception>();
        // mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Throws(new Exception());

        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 22 },
            TechStackList = new List<string> { "" }
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        //Assert.AreEqual(result, ApplicationResult.AutoRejected);
        result.Should().Be(ApplicationResult.AutoRejected);
    }

    [Test]
    public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");
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

        // Assert.AreEqual(result, ApplicationResult.AutoAccepted);
        result.Should().Be(ApplicationResult.AutoAccepted);
    }

    [Test]
    public void Application_WithInvalidIdentityNumber_TransferredToAutoAccepted()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 37 }
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        //Assert.AreEqual(ApplicationResult.TransferredToHR, result);
        result.Should().Be(ApplicationResult.TransferredToHR);
    }

    [Test]
    public void Application_WithOfficeLocation_TransferredToCTO()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();

        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Spain");
        mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 37 },
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        // Assert.AreEqual(ApplicationResult.TransferredToCTO, result);
        result.Should().Be(ApplicationResult.TransferredToCTO);
    }

    [Test]
    public void Application_WithOver50_ValidationModeToDetailed()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();
        //mockValidator.SetupProperty(i => i.ValidationMode);
        //SetupAllProperties en üstte olmalı sonra elle set edilen setuplar olmalı, yoksa elle set ettiklerimiz sonra kaybolur
        mockValidator.SetupAllProperties();
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Spain");
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 51 },
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        // Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
    }

    [Test]
    public void Application_WithNullApplicant_ThrowsArgumentNullException()
    {
        //Arrange

        var mockValidator = new Mock<IIdentityValidator>();

        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication();

        // Action
        // Exceptionları bu şekilde yakalayabiliyoruz. Beklentimiz de exception fırlatması 
        Action appResultAction = () => evaluator.Evaluate(form);

        //Assert

        appResultAction.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Application_WithDefaultValue_IsValidCalled()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();

        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 19, IdentityNumber = "1234" },
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), "IsValid Method should be called with 123");
    }
    
    [Test]
    public void Application_WithYoungAge_IsValidNeverCalled()
    {
        // Arrange
        var mockValidator = new Mock<IIdentityValidator>();

        mockValidator.DefaultValue = DefaultValue.Mock;
        mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Turkey");
        var evaluator = new ApplicationEvaluator(mockValidator.Object);

        var form = new JobApplication
        {
            Applicant = new Applicant { Age = 17 },
        };

        // Action

        var result = evaluator.Evaluate(form);

        //Assert

        // mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), Times.Never);
        // Aynı sonucu verir sadece count testini de bu şekilde yapabiliriz.
        mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), Times.Exactly(0));
    }
}