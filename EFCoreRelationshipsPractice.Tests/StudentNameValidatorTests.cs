using EFCoreRelationshipsPractice.Services;

namespace EFCoreRelationshipsPractice.Tests;

public class StudentNameValidatorTests
{
    private readonly StudentNameValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_WhenNameIsEmpty_ReturnsFalse(string? name)
    {
        bool result = _validator.IsValid(name!);

        Assert.False(result);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void IsValid_WhenNameIsTooShort_ReturnsFalse(string name)
    {
        bool result = _validator.IsValid(name);

        Assert.False(result);
    }

    [Theory]
    [InlineData("Thanuja")]
    [InlineData("Arun")]
    [InlineData("Priya")]
    public void IsValid_WhenNameIsValid_ReturnsTrue(string name)
    {
        bool result = _validator.IsValid(name);

        Assert.True(result);
    }
}