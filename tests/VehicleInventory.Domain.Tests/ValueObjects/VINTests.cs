using FluentAssertions;
using VehicleInventory.Domain.ValueObjects;
using Xunit;

namespace VehicleInventory.Domain.Tests.ValueObjects;

public class VINTests
{
    [Fact]
    public void Create_Valid17CharVin_ReturnsSuccess()
    {
        var result = VIN.Create("1HGBH41JXMN109186");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Value.Should().Be("1HGBH41JXMN109186");
    }

    [Fact]
    public void Create_EmptyString_ReturnsFailure()
    {
        var result = VIN.Create(string.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WhitespaceString_ReturnsFailure()
    {
        var result = VIN.Create("   ");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_LessThan17Chars_ReturnsFailure()
    {
        var result = VIN.Create("1HGBH41JXMN1091");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("17");
    }

    [Fact]
    public void Create_MoreThan17Chars_ReturnsFailure()
    {
        var result = VIN.Create("1HGBH41JXMN109186X");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("17");
    }

    [Theory]
    [InlineData("1HGBH41JXMN10918I")] // contains I
    [InlineData("1HGBH41JXMN10918O")] // contains O
    [InlineData("1HGBH41JXMN10918Q")] // contains Q
    public void Create_VinWithInvalidChar_ReturnsFailure(string invalidVin)
    {
        var result = VIN.Create(invalidVin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("I, O, or Q");
    }

    [Fact]
    public void Create_InputWithLowercaseAndSpaces_TrimsAndUppercases()
    {
        var result = VIN.Create("  1hgbh41jxmn109186  ");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be("1HGBH41JXMN109186");
    }

    [Fact]
    public void Equals_TwoVinsWithSameValue_AreEqual()
    {
        var vin1 = VIN.Create("1HGBH41JXMN109186").Value!;
        var vin2 = VIN.Create("1HGBH41JXMN109186").Value!;

        vin1.Should().Be(vin2);
        vin1.Equals(vin2).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoVinsWithDifferentValues_AreNotEqual()
    {
        var vin1 = VIN.Create("1HGBH41JXMN109186").Value!;
        var vin2 = VIN.Create("2T1BURHE0JC043821").Value!;

        vin1.Should().NotBe(vin2);
    }
}
