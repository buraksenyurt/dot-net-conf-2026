using FluentAssertions;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ValidEmail_ReturnsSuccess()
    {
        var result = Email.Create("test@example.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_EmptyEmail_ReturnsFailure()
    {
        var result = Email.Create(string.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WhitespaceEmail_ReturnsFailure()
    {
        var result = Email.Create("   ");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_EmailExceeding254Chars_ReturnsFailure()
    {
        var longLocal = new string('a', 243);
        var longEmail = $"{longLocal}@example.com"; // 243 + 12 = 255 chars

        var result = Email.Create(longEmail);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("254");
    }

    [Fact]
    public void Create_EmailWithoutAtSign_ReturnsFailure()
    {
        var result = Email.Create("invalidemail.com");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("valid format");
    }

    [Fact]
    public void Create_UppercaseEmail_NormalizesToLowercase()
    {
        var result = Email.Create("Test@Example.COM");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_EmailWithLeadingTrailingSpaces_Trims()
    {
        var result = Email.Create("  user@example.com  ");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Equals_TwoEmailsWithSameValue_AreEqual()
    {
        var email1 = Email.Create("user@example.com").Value!;
        var email2 = Email.Create("user@example.com").Value!;

        email1.Should().Be(email2);
        email1.Equals(email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoEmailsWithDifferentValues_AreNotEqual()
    {
        var email1 = Email.Create("user1@example.com").Value!;
        var email2 = Email.Create("user2@example.com").Value!;

        email1.Should().NotBe(email2);
    }
}
