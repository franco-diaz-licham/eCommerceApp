namespace Backend.Src.Domain.ValueObjects;

[Owned]
public class ShippingAddress
{
    public ShippingAddress() { }
    public ShippingAddress(string line1, string? line2, string city, string state, string postalCode, string country)
    {
        Line1 = Validate(line1, "Line1");
        Line2 = ValidateOptional(line2);
        City = Validate(city, "City");
        State = Validate(state, "State");
        PostalCode = Validate(postalCode, "PostalCode");
        Country = Validate(country, "Country").ToUpperInvariant();
    }
    #region Properties

    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    #endregion

    #region Business logic
    public void SetLine1(string value) => Line1 = Validate(value, "Line1");
    public void SetLine2(string? value) => Line2 = ValidateOptional(value);
    public void SetCity(string value) => City = Validate(value, "City");
    public void SetState(string value) => State = Validate(value, "State");
    public void SetPostalCode(string value) => PostalCode = Validate(value, "PostalCode");
    public void SetCountry(string value) => Country = Validate(value, "Country").ToUpperInvariant();

    private static string Validate(string s, string field)
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

    private static string? ValidateOptional(string? s) => string.IsNullOrWhiteSpace(s) ? null : Validate(s, "Line2");

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}
