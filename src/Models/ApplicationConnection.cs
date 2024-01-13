using Newtonsoft.Json;
using WinAppCommunity.Sdk.Models.JsonConverters;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents an application connection.
/// </summary>
/// <param name="ConnectionName">The name of the connection.</param>
[JsonConverter(typeof(ApplicationConnectionJsonConverter))]
public abstract record ApplicationConnection(string ConnectionName);

/// <summary>
/// Represents an application connection to Discord.
/// </summary>
/// <param name="DiscordId"></param>
/// <param name="ConnectionName">The name of the connection.</param>
public record DiscordConnection(string DiscordId, string ConnectionName = "discord") : ApplicationConnection(ConnectionName);

/// <summary>
/// Represents an application connection to an Email address.
/// </summary>
/// <param name="Email">An email address.</param>
/// <param name="ConnectionName">The name of the connection.</param>
public record EmailConnection(string Email, string ConnectionName = "email") : ApplicationConnection(ConnectionName);