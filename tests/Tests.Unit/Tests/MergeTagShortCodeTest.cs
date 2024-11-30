using static App.Server.Notification.Application.Domain.Types.MergeTagShortCode;

namespace App.Server.Notification.Tests.Unit.Tests;

/// <summary>
/// Tests for <see cref="MergeTag"/>.
/// </summary>
public class MergeTagShortCodeTest
{
    [Theory]
    [InlineData("value", "value")]
    [InlineData("VAlUe", "value")]
    [InlineData(" value ", "value")]
    [InlineData("value value", "value_value")]
    [InlineData(" value value value ", "value_value_value")]
    [InlineData(" value.name ", "value.name")]
    [InlineData(" value.name.value ", "value.name.value")]
    public void Generate_WithValidValue_Returns_MergeTagShortCode(
        string value,
        string expectedValueWithoutMarker
    )
    {
        // Arrange
        var expectedValue = $"{MergeTagBeginMarker}{expectedValueWithoutMarker}{MergeTagEndMarker}";

        // Act
        var shortCode = Generate(value);

        // Assert
        Assert.Equal(expectedValue, shortCode.Value);
    }

    [Theory]
    [InlineData("value")]
    [InlineData("some_value")]
    [InlineData("value_name_value")]
    [InlineData("value.name")]
    [InlineData("value.name.value")]
    [InlineData("value.name_value")]
    public void Validate_ShortCode_Works(string value)
    {
        // Arrange
        var code = MergeTagBeginMarker + value + MergeTagEndMarker;

        // Act
        var result = new MergeTagShortCode(code);

        // Assert
        Assert.Equal(code, result.Value);
    }

    [Theory]
    [InlineData("value")] // no markers
    [InlineData("some_value")] // no markers
    [InlineData("[value]")] // wrong markers
    [InlineData("{value}")] // wrong markers
    [InlineData("[[[value]]]")] // multiple wrong markers
    [InlineData(MergeTagBeginMarker + MergeTagBeginMarker + "value}}")] // factor of the markers e.g. {{ -> {{{{
    [InlineData("{{value" + MergeTagBeginMarker + MergeTagEndMarker)] // factor of the markers e.g. }} -> }}}}
    [InlineData(MergeTagBeginMarker + "value}}}")] // only one correct marker
    [InlineData("{{{value" + MergeTagEndMarker)] // only one correct marker
    [InlineData(MergeTagBeginMarker + "val}ue" + MergeTagEndMarker)] // disallowed characters '}'
    [InlineData(MergeTagBeginMarker + "val{ue" + MergeTagEndMarker)] // disallowed characters '{'
    [InlineData(MergeTagBeginMarker + "va{{}}lue" + MergeTagEndMarker)] // disallowed characters '{{' and '}}'
    [InlineData(MergeTagBeginMarker + "va{{asdad}}lue" + MergeTagEndMarker)] // disallowed characters '{{' and '}}'
    [InlineData(MergeTagBeginMarker + "val'ue" + MergeTagEndMarker)] // disallowed characters "'"
    [InlineData(MergeTagBeginMarker + "val`ue" + MergeTagEndMarker)] // disallowed characters "`"
    [InlineData(MergeTagBeginMarker + "val}^ue" + MergeTagEndMarker)] // disallowed characters "^"
    [InlineData(MergeTagBeginMarker + "val ue" + MergeTagEndMarker)] // disallowed characters " "
    [InlineData(MergeTagBeginMarker + """v"al"ue""" + MergeTagEndMarker)] // disallowed characters '"'
    [InlineData(MergeTagBeginMarker + "vàlue" + MergeTagEndMarker)] // disallowed characters 'à'
    [InlineData(MergeTagBeginMarker + "välue" + MergeTagEndMarker)] // disallowed characters 'ä'
    [InlineData(MergeTagBeginMarker + "value$" + MergeTagEndMarker)] // disallowed characters '$'
    [InlineData(MergeTagBeginMarker + "value-" + MergeTagEndMarker)] // disallowed characters '-'
    public void Validate_ShortCode_Throws(string code)
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<ArgumentException>(() => new MergeTagShortCode(code));
    }
}
