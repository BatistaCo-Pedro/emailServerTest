namespace App.Server.Notification.Tests.Unit.Tests;

/// <summary>
/// Test class for <see cref="CustomMergeTag"/>.
/// </summary>
public class CustomMergeTagTest
{
    [Theory]
    [MemberData(
        nameof(CustomMergeTagTestData.GetDataForCreationByString),
        MemberType = typeof(CustomMergeTagTestData)
    )]
    public void Initialize_CustomMergeTag_With_JsonValue_ValueType_Matches_Type(
        Type type,
        object value,
        CustomMergeTag customMergeTag
    )
    {
        // Arrange

        // Act

        // Assert
        Assert.Equal(type, customMergeTag.Type);
        Assert.Equal(type.FullName ?? type.Name, customMergeTag.TypeName);
        Assert.Equal(value, customMergeTag.Value);
    }

    [Theory]
    [MemberData(
        nameof(CustomMergeTagTestData.GetDataForCreationByObject),
        MemberType = typeof(CustomMergeTagTestData)
    )]
    public void Initialize_CustomMergeTag_With_Object_Matches_Type(
        Type type,
        object value,
        CustomMergeTag customMergeTag
    )
    {
        // Arrange

        // Act

        // Assert
        Assert.Equal(type, customMergeTag.Type);
        Assert.Equal(type.FullName ?? type.Name, customMergeTag.TypeName);
        Assert.Equal(value, customMergeTag.Value);
    }

    [Theory]
    [MemberData(
        nameof(CustomMergeTagTestData.GetDataForCreationByObject),
        MemberType = typeof(CustomMergeTagTestData)
    )]
    public void Initialize_CustomMergeTag_JsonConstructor_Matches_Code_Initialized(
        Type _1,
        object _2,
        CustomMergeTag customMergeTag
    )
    {
        // Arrange
        var serialized = JsonSerializer.Serialize(customMergeTag);
        var deserialized = JsonSerializer.Deserialize<CustomMergeTag>(serialized)!;

        // Act

        // Assert
        Assert.Equal(deserialized.Name, customMergeTag.Name);
        Assert.Equal(deserialized.Type, customMergeTag.Type);
        Assert.Equal(deserialized.TypeName, customMergeTag.TypeName);
        Assert.Equal(deserialized.Value, customMergeTag.Value);
    }
}
