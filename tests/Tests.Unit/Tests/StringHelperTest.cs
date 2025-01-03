namespace App.Server.Notification.Tests.Unit.Tests;

/// <summary>
/// Tests for <see cref="StringHelper"/>.
/// </summary>
public class StringHelperTest
{
    [Fact]
    public void Parse_Should_Return_Parsed_Value()
    {
        // Arrange
        var dateTimeString = "2021-01-01T00:00:05Z";
        var dateOnlyString = "2021-01-01";
        var timeOnlyString = "00:00:00";
        var doubleString = "123.45";
        var guidString = "12345678-1234-1234-1234-123456789012";
        var boolString = "true";
        var stringValue = "test";

        // Order of the types matters - DateOnly and TimeOnly are also DateTimes,
        // if you wish to get them as DateOnly or TimeOnly, they need to come before DateTime.
        var stringToTypeMap = new Dictionary<Type, string>()
        {
            { typeof(DateOnly), dateOnlyString },
            { typeof(TimeOnly), timeOnlyString },
            { typeof(DateTime), dateTimeString },
            { typeof(Guid), guidString },
            { typeof(double), doubleString },
            { typeof(bool), boolString },
            { typeof(string), stringValue },
        };

        // Act

        // Assert
        foreach (var keyValuePair in stringToTypeMap)
        {
            Assert.Equal(
                keyValuePair.Key,
                StringHelper
                    .Parse(keyValuePair.Value, keyValuePair.Key)
                    .GetType()
            );
        }
    }

    [Fact]
    public void Parse_Should_Return_String_On_Unsuccessful_Cast()
    {
        // Arrange
        var objectString =
            @"
            {
                ""key"": ""value""
                ""key2"": 123
            }";
        var arrayString = "[1, 2, 3]";
        var nullString = "null";

        string[] unsupportedValues = [objectString, arrayString, nullString];

        // Act

        // Assert
        foreach (var unsupportedValue in unsupportedValues)
        {
            Assert.Equal(
                typeof(string),
                StringHelper
                    .Parse(
                        unsupportedValue,
                        Assembly.GetExecutingAssembly().GetTypes().ToImmutableArray()
                    )
                    .GetType()
            );
        }
    }

    [Fact]
    public void Parse_Should_Return_String_On_Unsupported_Cast()
    {
        // Arrange
        var dateTimeString = "2021-01-01T00:00:05Z";
        var doubleString = "123.45";
        var guidString = "12345678-1234-1234-1234-123456789012";
        var boolString = "true";
        var stringValue = "test";

        var supportedStringToTypeMap = new Dictionary<Type, string>()
        {
            { typeof(DateTime), dateTimeString },
            { typeof(double), doubleString },
        };

        var unsupportedStringToTypeMap = new Dictionary<Type, string>()
        {
            { typeof(Guid), guidString },
            { typeof(bool), boolString },
            { typeof(string), stringValue },
        };

        // Act

        // Assert
        foreach (var keyValuePair in supportedStringToTypeMap)
        {
            Assert.Equal(
                keyValuePair.Key,
                StringHelper
                    .Parse(keyValuePair.Value, supportedStringToTypeMap.Keys.ToImmutableArray())
                    .GetType()
            );
        }

        foreach (var keyValuePair in unsupportedStringToTypeMap)
        {
            Assert.Equal(
                typeof(string),
                StringHelper
                    .Parse(keyValuePair.Value, supportedStringToTypeMap.Keys.ToImmutableArray())
                    .GetType()
            );
        }
    }
}
