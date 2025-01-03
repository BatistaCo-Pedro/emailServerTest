namespace App.Server.Notification.Application.Domain.Types;

[Serializable]
[JsonConverter(typeof(StringToWrapperJsonConverter<Base64String>))]
public record Base64String : NonEmptyString, IStringWrapper<Base64String>
{
    public Base64String(string value)
        : base(value)
    {
        if (!Base64.IsValid(value))
        {
            throw new ArgumentException("The value is not a valid Base64 string.");
        }
    }

    public new static Base64String Create(string value) => new(value);
}