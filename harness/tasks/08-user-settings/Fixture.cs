using System;

public sealed record UserSettings(string Theme, string Language, int PageSize)
{
    /// <summary>The settings used for users who have none saved.</summary>
    public static UserSettings Default { get; } = new("light", "en", 25);
}

public interface ISettingsCache
{
    /// <summary>Returns the cached settings for the user, or null on a cache miss.</summary>
    UserSettings? Get(Guid userId);

    /// <summary>Caches the settings for the user.</summary>
    void Set(Guid userId, UserSettings settings);
}

public interface ISettingsStore
{
    /// <summary>Returns the user's saved settings, or null when the user has never saved any.</summary>
    UserSettings? Load(Guid userId);
}
