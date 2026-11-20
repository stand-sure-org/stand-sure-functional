using System;

/// <summary>One parsed line of the uploaded CSV file. LineNumber is 1-based.</summary>
public sealed record ContactRow(int LineNumber, string Name, string Email);

public sealed record Contact(Guid Id, string Name, string Email);

/// <summary>Thrown by the store when a contact with the same email already exists.</summary>
public sealed class DuplicateContactException(string email) : Exception($"A contact with email {email} already exists.")
{
    public string Email { get; } = email;
}

public interface IContactStore
{
    /// <summary>Saves the contact and returns it with its generated id.</summary>
    /// <exception cref="DuplicateContactException">A contact with this email already exists.</exception>
    Contact Save(string name, string email);
}
