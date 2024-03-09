using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Linq;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;

namespace WinAppCommunity.Sdk.Models.JsonConverters;

internal static class ApplicationConnectionSerializationHelpers
{
    internal static JObject? WriteConnection(ApplicationConnection connection)
    {
        var jObject = new JObject();

        jObject.AddFirst(new JProperty("connectionName", new JValue(connection.ConnectionName)));

        if (connection is DiscordConnection discordConnection)
            jObject.AddFirst(new JProperty("discordId", new JValue(discordConnection.DiscordId)));

        if (connection is EmailConnection emailConnection)
            jObject.AddFirst(new JProperty("email", new JValue(emailConnection.Email)));

        return jObject;
    }

    internal static object? ReadConnection(JToken token, JsonSerializer serializer)
    {
        if (token.Type == JTokenType.Array)
        {
            var jsonArray = (JArray)token;
            return jsonArray.Select(jToken => ReadConnection(jToken, serializer)).PruneNull().FirstOrDefault();
        }

        if (token is JObject jObject)
            return ReadConnection(jObject, serializer);

        throw new NotSupportedException($"Token type {token.Type} is not supported.");
    }

    internal static ApplicationConnection? ReadConnection(JObject jObject, JsonSerializer serializer)
    {
        var connectionName = jObject["connectionName"]?.Value<string>();

        if (connectionName == "discord")
        {
            var discordId = jObject["discordId"]?.Value<string>();
            Guard.IsNotNull(discordId);

            return new DiscordConnection(discordId, connectionName);
        }

        if (connectionName == "email")
        {
            var email = jObject["email"]?.Value<string>();
            if (email is null)
                return null;

            return new EmailConnection(email, connectionName);
        }

        return null;
    }
}

/// <summary>
/// A custom json convert for discriminating the various types of <see cref="ApplicationConnection"/>.
/// </summary>
public class ApplicationConnectionJsonConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(ApplicationConnection))
            return true;

        var arrayElement = objectType.GetElementType();
        if (objectType.IsArray && arrayElement == typeof(ApplicationConnection))
            return true;

        return false;
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        if (reader.TokenType == JsonToken.PropertyName)
        {
            return reader.Value;
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
            var jobj = JObject.Load(reader);
            return ApplicationConnectionSerializationHelpers.ReadConnection(jobj, serializer);
        }

        if (reader.TokenType == JsonToken.StartArray)
        {
            var jarray = JArray.Load(reader);
            return ApplicationConnectionSerializationHelpers.ReadConnection(jarray, serializer);
        }

        throw new NotSupportedException($"Token type {reader.TokenType} is not supported.");
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            return;

        if (value is ApplicationConnection connection)
        {
            var jObject = ApplicationConnectionSerializationHelpers.WriteConnection(connection);
            jObject?.WriteTo(writer);
        }
        else if (value is ApplicationConnection[] connections)
        {
            var jArray = new JArray();
            foreach (var item in connections)
            {
                var jObject = ApplicationConnectionSerializationHelpers.WriteConnection(item);

                if (jObject is null)
                    jArray.Add(JValue.CreateNull());
                else
                    jArray.Add(jObject);
            }

            jArray.WriteTo(writer);
        }
        else
        {
            throw new NotSupportedException($"Value type {value.GetType()} is not supported.");
        }
    }
}
