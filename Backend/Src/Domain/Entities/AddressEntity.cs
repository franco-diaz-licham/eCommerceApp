namespace Backend.Src.Domain.Entities;

public sealed class AddressEntity : BaseEntity
{
    private AddressEntity() { }

    public AddressEntity(string line1, string? line2, string city, string state, string postalCode, string country)
    {
        SetLine1(line1);
        SetLine2(line2);
        SetCity(city);
        SetState(state);
        SetPostalCode(postalCode);
        SetCountry(country);
    }

    #region Properties
    public string Line1 { get; private set; } = default!;
    public string? Line2 { get; private set; }
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    #endregion

    #region Business logic
    public void Update(string line1, string? line2, string city, string state, string postalCode, string country)
    {
        SetLine1(line1);
        SetLine2(line2);
        SetCity(city);
        SetState(state);
        SetPostalCode(postalCode);
        SetCountry(country);
    }

    public void SetLine1(string value)
    {
        Line1 = Validate(value, nameof(Line1));
    }
    
    public void SetLine2(string? value)
    {
        Line2 = ValidateOptional(value);
    }

    public void SetCity(string value)
    {
        City = Validate(value, nameof(City));
    }

    public void SetState(string value)
    {
        State = Validate(value, nameof(State));
    }

    public void SetPostalCode(string value)
    {
        PostalCode = Validate(value, nameof(PostalCode));
    }

    public void SetCountry(string value)
    {
        Country = Validate(value, nameof(Country)).ToUpperInvariant();
    }

    private static string Validate(string s, string field)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException($"{field} is required.");

        var v = CollapseSpaces(s.Trim());
        return v.Length switch
        {
            > 128 when field is nameof(Line1) or nameof(Line2) => throw new ArgumentException($"{field} too long."),
            > 12 when field is nameof(City) or nameof(State) => throw new ArgumentException($"{field} too long."),
            > 4 when field is nameof(PostalCode) => throw new ArgumentException($"{field} too long."),
            > 56 when field is nameof(Country) => throw new ArgumentException($"{field} too long."),
            _ => v
        };
    }

    private static string? ValidateOptional(string? s) => string.IsNullOrWhiteSpace(s) ? null : Validate(s, nameof(Line2));
    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}

