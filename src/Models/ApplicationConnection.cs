namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents an application connection.
/// </summary>
/// <param name="connectionName">The name of the connection.</param>
public abstract record ApplicationConnection(string connectionName);

/// <summary>
/// Represents an application connection to Discord.
/// </summary>
/// <param name="discordId"></param>
/// <param name="connectionName">The name of the connection.</param>
public record DiscordConnection(string discordId, string connectionName = "discord") : ApplicationConnection(connectionName);

/// <summary>
/// Represents an application connection to an Email address.
/// </summary>
/// <param name="email">An email address.</param>
/// <param name="connectionName">The name of the connection.</param>
public record EmailConnection(string email, string connectionName = "email") : ApplicationConnection(connectionName);