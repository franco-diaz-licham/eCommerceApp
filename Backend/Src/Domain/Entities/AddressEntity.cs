namespace Backend.Src.Domain.Entities;

public class AddressEntity : BaseEntity
{
    protected AddressEntity() { }
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
    #endregion


    #region Business logic
    public void SetLine1(string value) => Line1 = Validate(value, "Line1");
    public void SetLine2(string? value) => Line2 = ValidateOptional(value);
    public void SetCity(string value) => City = Validate(value, "City");
    public void SetState(string value) => State = Validate(value, "State");
    public void SetPostalCode(string value) => PostalCode = Validate(value, "PostalCode");
    public void SetCountry(string value) => Country = Validate(value, "Country").ToUpperInvariant();

    protected static string Validate(string s, string field)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException($"{field} is required.");
        var v = CollapseSpaces(s.Trim());
        return v.Length switch
        {
            > 128 when field is "Line1" or "Line2" => throw new ArgumentException($"{field} too long."),
            > 12 when field is "City" or "State" => throw new ArgumentException($"{field} too long."),
            > 4 when field is "PostalCode" => throw new ArgumentException($"{field} too long."),
            > 56 when field is "Country" => throw new ArgumentException($"{field} too long."),
            _ => v
        };
    }

    protected static string? ValidateOptional(string? s) => string.IsNullOrWhiteSpace(s) ? null : Validate(s, "Line2");

    protected static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}

