namespace Backend.Src.Domain.Entities;

public class UserEntity : IdentityUser
{
    public UserEntity() { }
    public UserEntity(string username, string email)
    {
        SetUsername(username);
        SetEmail(email);
        IsActive = true;
    }

    #region Properties
    public int? AddressId { get; private set; }
    public AddressEntity? Address { get; private set; }
    public bool IsActive { get; private set; } = true;
    #endregion

    #region Business Logic

    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username), "Username is required.");
        if (username.Length > 50) throw new ArgumentException("Username too long (max 50 chars).", nameof(username));

        UserName = username.Trim();
        NormalizedUserName = UserName.ToUpperInvariant();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email), "Email is required.");
        if (!email.Contains("@")) throw new ArgumentException("Invalid email address.", nameof(email));

        Email = email.Trim();
        NormalizedEmail = Email.ToUpperInvariant();
    }

    public void AssignAddress(AddressEntity address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));

        Address = address;
        AddressId = address.Id;
    }

    public void ClearAddress()
    {
        Address = null;
        AddressId = null;
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    #endregion
}
