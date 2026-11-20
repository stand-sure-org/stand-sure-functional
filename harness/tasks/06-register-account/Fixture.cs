using System;

public sealed record SignupForm(string Username, string Email, string Password, int Age);

public sealed record Account(Guid Id, string Username, string Email);

public interface IAccountStore
{
    /// <summary>Creates the account and returns it with its generated id.</summary>
    Account Create(string username, string email, string password);
}
