namespace App.Server.Notification.Tests.Unit.TestData;

/// <summary>
/// Test data for <see cref="CustomMergeTagTest"/>.
/// </summary>
public static class CustomMergeTagTestData
{
    public static IEnumerable<object[]> GetDataForCreationByString()
    {
        yield return
        [
            typeof(DateTime),
            DateTime.MinValue,
            new CustomMergeTag(
                nameof(DateTime),
                DateTime.MinValue.ToString(CultureInfo.InvariantCulture)
            ),
        ];
        yield return
        [
            typeof(TimeOnly),
            TimeOnly.MinValue,
            new CustomMergeTag(
                nameof(TimeOnly),
                TimeOnly.MinValue.ToString(CultureInfo.InvariantCulture)
            ),
        ];
        yield return
        [
            typeof(DateOnly),
            DateOnly.MinValue,
            new CustomMergeTag(
                nameof(DateOnly),
                DateOnly.MinValue.ToString(CultureInfo.InvariantCulture)
            ),
        ];
        yield return
        [
            typeof(Guid),
            Guid.Parse("b34384c6-6c8b-4cb0-85b1-f117a76e35f8"),
            new CustomMergeTag(
                nameof(Guid),
                Guid.Parse("b34384c6-6c8b-4cb0-85b1-f117a76e35f8")
                    .ToString(null, CultureInfo.InvariantCulture)
            ),
        ];
        yield return
        [
            typeof(double),
            123.45,
            new CustomMergeTag(nameof(Double), 123.45.ToString(CultureInfo.InvariantCulture)),
        ];
        yield return
        [
            typeof(bool),
            true,
            new CustomMergeTag(nameof(Boolean), true.ToString(CultureInfo.InvariantCulture)),
        ];
        yield return [typeof(string), "test", new CustomMergeTag(nameof(String), "test")];
    }

    public static IEnumerable<object[]> GetDataForCreationByObject()
    {
        yield return
        [
            typeof(DateTime),
            DateTime.MinValue,
            new CustomMergeTag(nameof(DateTime), DateTime.MinValue),
        ];
        yield return
        [
            typeof(TimeOnly),
            TimeOnly.MinValue,
            new CustomMergeTag(nameof(TimeOnly), TimeOnly.MinValue),
        ];
        yield return
        [
            typeof(DateOnly),
            DateOnly.MinValue,
            new CustomMergeTag(nameof(DateOnly), DateOnly.MinValue),
        ];
        yield return
        [
            typeof(Guid),
            Guid.Parse("b34384c6-6c8b-4cb0-85b1-f117a76e35f8"),
            new CustomMergeTag(nameof(Guid), Guid.Parse("b34384c6-6c8b-4cb0-85b1-f117a76e35f8")),
        ];
        yield return [typeof(double), 123.45, new CustomMergeTag(nameof(Double), 123.45)];
        yield return [typeof(bool), true, new CustomMergeTag(nameof(Boolean), true)];
        yield return [typeof(string), "test", new CustomMergeTag(nameof(String), (object)"test")];
    }
}
