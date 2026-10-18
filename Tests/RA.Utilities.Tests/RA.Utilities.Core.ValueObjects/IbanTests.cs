using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.ValueObjects;

namespace RA.Utilities.Tests.RA.Utilities.Core.ValueObjects;

/// <summary>
/// Contains unit tests for the <see cref="IBAN"/> value object.
/// </summary>
public class IbanTests
{
    // =================================================================
    // Constructor and normalization
    // =================================================================

    /// <summary>
    /// Provides valid IBANs and their expected normalized form.
    /// </summary>
    public static TheoryData<string, string> ValidIbans => new()
    {
        { "GB82WEST12345698765432", "GB82WEST12345698765432" },
        { "gb82west12345698765432", "GB82WEST12345698765432" },
        { "  gb82 west 1234 5698 7654 32  ", "GB82WEST12345698765432" }, // paper format groups
        { "GB82\u00A0WEST\u00A01234\u00A05698\u00A07654\u00A032", "GB82WEST12345698765432" }, // non-breaking spaces
        { "GB82-WEST-1234-5698-7654-32", "GB82WEST12345698765432" }, // hyphen separators
        { "GB82'WEST'1234'5698'7654'32", "GB82WEST12345698765432" }, // apostrophe separators
        { "GB82  WEST  1234 5698 7654 32", "GB82WEST12345698765432" }, // repeated separators collapse
        { "GB29NWBK60161331926819", "GB29NWBK60161331926819" },
        { "DE89 3704 0044 0532 0130 00", "DE89370400440532013000" },
        { "FR14 2004 1010 0505 0001 3M02 606", "FR1420041010050500013M02606" },
        { "BE68 5390 0754 7034", "BE68539007547034" },
        { "NL91 ABNA 0417 1643 00", "NL91ABNA0417164300" },
        { "NO93 8601 1117 947", "NO9386011117947" },
        { "MT84 MALT 0110 0001 2345 MTLC AST0 01S", "MT84MALT011000012345MTLCAST001S" },
        { "CH93 0076 2011 6238 5295 7", "CH9300762011623852957" },
        { "ES91 2100 0418 4502 0005 1332", "ES9121000418450200051332" },
        { "IT60 X054 2811 1010 0000 0123 456", "IT60X0542811101000000123456" },
        { "PT50 0002 0123 1234 5678 9015 4", "PT50000201231234567890154" },
        { "IE29 AIBK 9311 5212 3456 78", "IE29AIBK93115212345678" },
        { "AT61 1904 3002 3457 3201", "AT611904300234573201" },
        { "DK50 0040 0440 1162 43", "DK5000400440116243" },
        { "SE74 5000 0000 5839 8257 4660", "SE7450000000583982574660" },
        { "FI21 1234 5600 0007 85", "FI2112345600000785" },
        { "PL61 1090 1014 0000 0712 1981 2874", "PL61109010140000071219812874" },
        { "CZ65 0800 0000 1920 0014 5399", "CZ6508000000192000145399" },
        { "GR16 0110 1250 0000 0001 2300 695", "GR1601101250000000012300695" },
        { "IL62 0108 0000 0009 9999 999", "IL620108000000099999999" },
        { "SA03 8000 0000 6080 1016 7519", "SA0380000000608010167519" },
        { "TR33 0006 1005 1978 6457 8413 26", "TR330006100519786457841326" },
        { "BR97 0036 0305 0000 1000 9795 493P 1", "BR9700360305000010009795493P1" },
        { "QA58 DOHB 0000 1234 5678 90AB CDEF G", "QA58DOHB00001234567890ABCDEFG" },
        { "KZ86 125K ZT50 0410 0100", "KZ86125KZT5004100100" },
        { "XK05 1212 0123 4567 8906", "XK051212012345678906" },
        { "SI56 2633 0001 2039 086", "SI56263300012039086" },
        { "SK31 1200 0000 1987 4263 7541", "SK3112000000198742637541" },
        { "HR12 1001 0051 8630 0016 0", "HR1210010051863000160" },
        { "BG80 BNBG 9661 1020 3456 78", "BG80BNBG96611020345678" },
        { "RO49 AAAA 1B31 0075 9384 0000", "RO49AAAA1B31007593840000" },
        { "CY17 0020 0128 0000 0012 0052 7600", "CY17002001280000001200527600" },
        { "IS14 0159 2600 7654 5510 7303 39", "IS140159260076545510730339" },
        { "LT12 1000 0111 0100 1000", "LT121000011101001000" },
        { "LV80 BANK 0000 4351 9500 1", "LV80BANK0000435195001" },
        { "EE38 2200 2210 2014 5685", "EE382200221020145685" },
        { "LU28 0019 4006 4475 0000", "LU280019400644750000" },
        { "MK07 2501 2000 0058 984", "MK07250120000058984" },
        { "ME25 5050 0001 2345 6789 51", "ME25505000012345678951" },
        { "RS35 2600 0560 1001 6113 79", "RS35260005601001611379" },
        { "MD24 AG00 0225 1000 1310 4168", "MD24AG000225100013104168" },
        { "AE07 0331 2345 6789 0123 456", "AE070331234567890123456" },
        { "VG96 VPVG 0000 0123 4567 8901", "VG96VPVG0000012345678901" },
        { "JO94 CBJO 0010 0000 0000 0131 0003 02", "JO94CBJO0010000000000131000302" },
        { "KW81 CBKU 0000 0000 0000 1234 5601 01", "KW81CBKU0000000000001234560101" },
        { "LB62 0999 0000 0001 0019 0122 9114", "LB62099900000001001901229114" },
        { "HU42 1177 3016 1111 1018 0000 0000", "HU42117730161111101800000000" },
    };

    [Theory]
    [MemberData(nameof(ValidIbans))]
    public void Constructor_ValidInput_ShouldNormalizeValue(string input, string expected)
    {
        // Act
        var iban = new IBAN(input);

        // Assert
        iban.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_MissingInput_ShouldThrowRequiredError(string? input)
    {
        // Act
        Func<IBAN> act = () => new IBAN(input!);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.ErrorCode.Should().Be(BaseResponseCode.BadRequest);
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.IbanRequired);
        exception.Errors[0].PropertyName.Should().Be("IBAN");
    }

    [Theory]
    [InlineData("GB")]                                // shorter than the 13-character minimum
    [InlineData("G8")]                                // shorter than the 13-character minimum
    [InlineData("GB8212345678")]                      // only 8 characters after the header
    [InlineData("GB82WEST123456987654321234567890123")] // more than 30 characters after the header
    [InlineData("G182WEST12345698765432")]            // country code must be letters
    [InlineData("8B82WEST12345698765432")]            // country code must be letters
    [InlineData("GB8AWEST12345698765432")]            // check digits must be digits
    [InlineData("GB82_WEST12345698765432")]           // underscore is not a valid separator
    [InlineData("GB82WEST1234569876543!")]            // punctuation is not alphanumeric
    [InlineData("GB82WEST1234569876 5432!!")]         // punctuation is not alphanumeric
    public void Constructor_InvalidStructure_ShouldThrowNotValidError(string input)
    {
        // Act
        Func<IBAN> act = () => new IBAN(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.IbanNotValid);
        exception.Errors[0].ErrorMessage.Should().Be(BaseErrorMessage.IbanNotValid);
        exception.Errors[0].AttemptedValue.Should().Be(Compact(input));
        exception.Errors[0].ExpectedValue.Should().BeOfType<string>().Which.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("GB81WEST12345698765432", "82")] // wrong check digits
    [InlineData("GB00WEST12345698765432", "82")] // 00 is outside the 02-98 range
    [InlineData("GB99WEST12345698765432", "82")] // 99 is outside the 02-98 range
    [InlineData("DE88370400440532013000", "89")]
    [InlineData("US82WEST12345698765432", "02")] // structurally valid, but not the check digits for US
    public void Constructor_InvalidCheckDigits_ShouldThrowCheckDigitsErrorWithExpectedValue(string input, string expectedCheckDigits)
    {
        // Act
        Func<IBAN> act = () => new IBAN(input);

        // Assert
        BadRequestException exception = act.Should().Throw<BadRequestException>().Which;
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].ErrorCode.Should().Be(BaseErrorCode.IbanCheckDigits);
        exception.Errors[0].ErrorMessage.Should().Be(BaseErrorMessage.IbanCheckDigits);
        exception.Errors[0].AttemptedValue.Should().Be(input);
        exception.Errors[0].ExpectedValue.Should().Be(expectedCheckDigits);
    }

    [Fact]
    public void Constructor_CountryOutsideTheRegistry_ShouldBeAccepted()
    {
        // Arrange — validation is structural, so a country code with no entry in the historic
        // IBAN formats table is accepted as long as the check digits are correct
        string iban = CreateIban("SN", "123456789012345678901234"); // 28-character SN-like IBAN

        // Act
        var parsed = new IBAN(iban);

        // Assert
        parsed.Value.Should().Be(iban);
        parsed.CountryCode.Should().Be("SN");
    }

    [Theory]
    [InlineData("SN", "123456789")]                      // 13 characters, the shortest structure
    [InlineData("SN", "123456789012345678901234567890")] // 34 characters, the longest structure
    public void Constructor_StructuralLengthBoundaries_ShouldBeAccepted(string countryCode, string bban)
    {
        // Arrange
        string iban = CreateIban(countryCode, bban);

        // Act
        var parsed = new IBAN(iban);

        // Assert
        parsed.Value.Should().Be(iban);
    }

    // =================================================================
    // Country code
    // =================================================================

    [Theory]
    [InlineData("GB82WEST12345698765432", "GB")]
    [InlineData("gb82 west 1234 5698 7654 32", "GB")]
    [InlineData("DE89370400440532013000", "DE")]
    [InlineData("NO9386011117947", "NO")]
    [InlineData("MT84 MALT 0110 0001 2345 MTLC AST0 01S", "MT")]
    public void CountryCode_ShouldReturnTheTwoLetterCountryPrefix(string input, string expected)
    {
        // Act
        var iban = new IBAN(input);

        // Assert
        iban.CountryCode.Should().Be(expected);
    }

    // =================================================================
    // Equality
    // =================================================================

    [Fact]
    public void Ibans_WithSameNormalizedValue_ShouldBeEqual()
    {
        // Act & Assert — casing and paper-format separators are normalized away
        new IBAN(" gb82 west 1234 5698 7654 32 ").Should().Be(new IBAN("GB82WEST12345698765432"));
        (new IBAN("gb82west12345698765432") == new IBAN("GB82WEST12345698765432")).Should().BeTrue();
        new IBAN("GB82-WEST-1234-5698-7654-32").Should().Be(new IBAN("GB82WEST12345698765432"));
    }

    [Fact]
    public void Ibans_WithDifferentValues_ShouldNotBeEqual()
    {
        // Act & Assert
        (new IBAN("GB82WEST12345698765432") == new IBAN("GB29NWBK60161331926819")).Should().BeFalse();
    }

    // =================================================================
    // Parse and TryParse
    // =================================================================

    [Fact]
    public void Parse_ValidInput_ShouldReturnNormalizedIban()
    {
        // Act
        var iban = IBAN.Parse("gb82 west 1234 5698 7654 32", provider: null);

        // Assert
        iban.Value.Should().Be("GB82WEST12345698765432");
    }

    [Fact]
    public void Parse_InvalidInput_ShouldThrowBadRequestException()
    {
        // Act
        Func<IBAN> act = () => IBAN.Parse("GB82", provider: null);

        // Assert
        act.Should().Throw<BadRequestException>();
    }

    [Fact]
    public void TryParse_ValidInput_ShouldReturnTrueAndIban()
    {
        // Act
        bool success = IBAN.TryParse("gb82west12345698765432", provider: null, out IBAN result);

        // Assert
        success.Should().BeTrue();
        result.Should().Be(new IBAN("GB82WEST12345698765432"));
    }

    [Fact]
    public void TryParse_InvalidInput_ShouldReturnFalseAndNull()
    {
        // Act
        bool success = IBAN.TryParse("GB81WEST12345698765432", provider: null, out IBAN result);

        // Assert
        success.Should().BeFalse();
        result.Should().BeNull();
    }

    // =================================================================
    // Conversions
    // =================================================================

    [Fact]
    public void ImplicitConversionToString_ShouldReturnValue()
    {
        // Act
        string value = new IBAN("GB82WEST12345698765432");

        // Assert
        value.Should().Be("GB82WEST12345698765432");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Act & Assert
        new IBAN("GB82WEST12345698765432").ToString().Should().Be("GB82WEST12345698765432");
    }

    // =================================================================
    // JSON serialization (via ValueObjectJsonConverter<IBAN>)
    // =================================================================

    [Fact]
    public void Serialize_ShouldWritePlainString()
    {
        // Act
        string json = JsonSerializer.Serialize(new IBAN("GB82WEST12345698765432"));

        // Assert
        json.Should().Be("\"GB82WEST12345698765432\"");
    }

    [Fact]
    public void Deserialize_ValidString_ShouldCreateNormalizedIban()
    {
        // Act
        IBAN? iban = JsonSerializer.Deserialize<IBAN>("\"gb82 west 1234 5698 7654 32\"");

        // Assert
        iban.Should().Be(new IBAN("GB82WEST12345698765432"));
    }

    [Fact]
    public void Deserialize_InvalidString_ShouldThrowJsonException()
    {
        // Act
        Func<IBAN?> act = () => JsonSerializer.Deserialize<IBAN>("\"GB81WEST12345698765432\"");

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void RoundTrip_ShouldPreserveEquality()
    {
        // Arrange
        var original = new IBAN("GB82WEST12345698765432");

        // Act
        IBAN? deserialized = JsonSerializer.Deserialize<IBAN>(JsonSerializer.Serialize(original));

        // Assert
        deserialized.Should().Be(original);
    }

    // =================================================================
    // Helpers
    // =================================================================

    /// <summary>
    /// Removes the grouping separators the value object strips during normalization.
    /// </summary>
    private static string Compact(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (char character in value)
        {
            if (char.IsWhiteSpace(character) || character is '\'' or '\\' or '-')
                continue;

            builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Builds an IBAN with valid ISO 7064 mod-97 check digits for the given country code and BBAN.
    /// </summary>
    private static string CreateIban(string countryCode, string bban)
    {
        int remainder = 0;
        foreach (char character in bban + countryCode + "00")
        {
            int value = char.IsAsciiDigit(character) ? character - '0' : character - 'A' + 10;
            int factor = value < 10 ? 10 : 100;
            remainder = (remainder * factor + value) % 97;
        }

        return string.Concat(
            countryCode,
            (98 - remainder).ToString("00", CultureInfo.InvariantCulture),
            bban);
    }
}
