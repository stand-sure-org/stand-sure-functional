using System;

public sealed record User(Guid Id, string Email, string DisplayName);

public interface IUserStore
{
    /// <summary>True when a user with this email (compared case-insensitively) is already registered.</summary>
    bool EmailExists(string email);

    /// <summary>Saves a new user and returns it with its generated id.</summary>
    User Add(string email, string displayName);
}
