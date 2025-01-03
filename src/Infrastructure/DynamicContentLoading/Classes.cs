using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace App.Server.Notification.Infrastructure.DynamicContentLoading;

public record MergeTag(string Identifier, Location Location);

public class Classes(DbContext db, IHttpClientFactory httpClientFactory)
{
    public Dictionary<string, string> GetData(
        ImmutableHashSet<MergeTag> mergeTags,
        params object[] alreadyAvailableData
    )
    {
        var data = new Dictionary<string, string>();
        foreach (
            var o in alreadyAvailableData
                .Select(x =>
                    (
                        Property: x,
                        Identifier: x.GetType().GetCustomAttribute<DataIdentifierAttribute>()
                    )
                )
                .Where(x => x.Identifier is not null)
        )
        {
            foreach (
                var propertyInfo in o.GetType()
                    .GetProperties()
                    .Select(x =>
                        (
                            Property: x,
                            Identifier: x.GetType().GetCustomAttribute<DataIdentifierAttribute>()
                        )
                    )
                    .Where(x => x.Identifier is not null)
            )
            {
                // example
                // identifier = guest.name -> guest : parent, name : child

                foreach (var mergeTagIdentifier in mergeTags.Select(x => x.Identifier))
                {
                    if (mergeTagIdentifier == $"{o.Identifier}.{propertyInfo.Identifier}")
                    {
                        data.Add(
                            mergeTagIdentifier,
                            propertyInfo.Property.GetValue(o)!.ToString()!
                        );
                    }
                }
            }
        }

        if (data.Count == mergeTags.Count)
        {
            return data;
        }

        var missingTags = mergeTags.Where(x => !data.ContainsKey(x.Identifier));
        Load(data, missingTags.ToImmutableHashSet());
        return data;
    }

    private void Load(
        Dictionary<string, string> dataDictionary,
        ImmutableHashSet<MergeTag> mergeTags
    )
    {
        var places = mergeTags.Select(x => x.Location.Place);
        foreach (var locationPlace in places)
        {
            var t = locationPlace switch
            {
                Database database => database.Handle(db),
                HttpCall method => method.Handle(httpClientFactory),
                _ => throw new NotImplementedException(),
            };
        }
    }
}

public abstract class LocationPlace
{
    public void Handle() { }

    protected void Handle(object data) { }
}

public interface IServer;

public class Database : LocationPlace
{
    public object Handle(DbContext db)
    {
        var method = typeof(DbContext).GetMethod(nameof(DbContext.Set)).MakeGenericMethod(typeof(int));
        var set = method.Invoke(db, null)!;

        return null!;
    }
}

public enum PmsAllowedParameters
{
    None = 0,
    ReservationToken = 1,
    ReservationTokens = 2,
    GuestTokens,
}

public class HttpCall : LocationPlace
{
    private const char Separator = ':';

    [MemberNotNull(nameof(Url), nameof(Method))]
    public static HttpCall FromString(string httpCall)
    {
        var parts = httpCall.Split(Separator);
        return new HttpCall(parts[0], parts[1]);
    }

    public HttpCall() { }

    private HttpCall(string url, string method)
    {
        Url = new Uri(url);
        Method = new HttpMethod(method);
    }

    public Uri? Url { get; private set; }

    public HttpMethod? Method { get; private set; }

    public object Handle(IHttpClientFactory htpClientFactory)
    {
        var client = htpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        var request = new HttpRequestMessage(Method, Url) { };
        var data = client.Send(request);
        return JsonSerializer.Deserialize<object>(data.Content.ReadAsStringAsync().Result);
    }
}

public enum Server
{
    None = 0,
    CMS = 1,
    PMS = 2,
}

public record Location(Server Server, LocationPlace Place);

public class TestClass
{
    public void TestMethod()
    {
        var att = Attribute.GetCustomAttributes(typeof(TestDto), typeof(DataIdentifierAttribute));
        var t = typeof(TestDto);
        new DataLoader().Load(t);
    }
}

public class DataLoader
{
    public object Load(Type type)
    {
        return null;
    }
}

[DataIdentifier("TestDto")]
public class TestDto()
{
    [DataIdentifier("name")]
    public required string Name { get; set; }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class DataIdentifierAttribute : Attribute
{
    public DataIdentifierAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public string Identifier { get; }
}
