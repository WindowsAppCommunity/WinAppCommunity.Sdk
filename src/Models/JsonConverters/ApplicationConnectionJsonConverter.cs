using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using CommunityToolkit.Diagnostics;

namespace WinAppCommunity.Sdk.Models.JsonConverters;

/// <summary>
/// A custom json convert for discriminating the various types of <see cref="ApplicationConnection"/>.
/// </summary>
public class ApplicationConnectionJsonConverter : JsonConverter<ApplicationConnection>
{
    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    /// <inheritdoc/>
    public override ApplicationConnection? ReadJson(JsonReader reader, Type objectType, ApplicationConnection? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null)
            return null;

        JObject jsonObject = JObject.Load(reader);
        var connectionName = jsonObject["connectionName"]?.Value<string>();

        if (connectionName == "discord")
        {
            var discordId = jsonObject["discordId"]?.Value<string>();
            Guard.IsNotNull(discordId);

            return new DiscordConnection(discordId, connectionName);
        }

        if (connectionName == "email")
        {
            var email = jsonObject["email"]?.Value<string>();
            if (email is null)
                return null;

            return new EmailConnection(email, connectionName);
        }

        return null;
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, ApplicationConnection? value, JsonSerializer serializer)
    {
        if (value is null)
            return;

        var o = new JObject();

        if (value is ApplicationConnection connection)
            o.AddFirst(new JProperty("connectionName", new JValue(connection.ConnectionName)));

        if (value is DiscordConnection discordConnection)
            o.AddFirst(new JProperty("discordId", new JValue(discordConnection.DiscordId)));

        if (value is EmailConnection emailConnection)
            o.AddFirst(new JProperty("email", new JValue(emailConnection.Email)));

        o.WriteTo(writer);
    }
}
