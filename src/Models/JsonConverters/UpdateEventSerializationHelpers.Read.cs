﻿using System;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Ipfs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OwlCore.Extensions;
using WinAppCommunity.Sdk.Models.UpdateEvents;

namespace WinAppCommunity.Sdk.Models.JsonConverters;

internal static partial class UpdateEventSerializationHelpers
{
    internal static object? Read(JToken token, JsonSerializer serializer)
    {
        if (token.Type == JTokenType.Array)
        {
            var jsonArray = (JArray)token;
            return jsonArray.Select(jToken => Read(jToken, serializer)).PruneNull().FirstOrDefault();
        }

        if (token is JObject jObject)
            return Read(jObject, serializer);

        throw new NotSupportedException($"Token type {token.Type} is not supported.");
    }


    internal static WinAppCommunityUpdateEvent? Read(JObject jObject, JsonSerializer serializer)
    {
        var id = jObject["Id"]?.Value<string>();
        var eventId = jObject["EventId"]?.Value<string>();

        // Basic validation
        Guard.IsNotNullOrWhiteSpace(id);
        Guard.IsNotNullOrWhiteSpace(eventId);

        var userEvent = ReadUser(jObject, eventId, id, serializer);
        if (userEvent is not null)
            return userEvent;

        var projectEvent = ReadProject(jObject, eventId, id, serializer);
        if (projectEvent is not null)
            return projectEvent;

        var publisherEvent = ReadPublisher(jObject, eventId, id, serializer);
        if (publisherEvent is not null)
            return publisherEvent;

        return null;
    }

    internal static WinAppCommunityUpdateEvent? ReadUser(JObject jObject, string eventId, string id, JsonSerializer serializer)
    {
        return eventId switch
        {
            nameof(UserNameUpdateEvent) when jObject["Name"] is { } nameToken && nameToken.Value<string>() is { } value =>
                new UserNameUpdateEvent(id, value),

            nameof(UserMarkdownAboutMeUpdateEvent) when jObject["MarkdownAboutMe"] is { } markdownToken && markdownToken.Value<string>() is { } value =>
                new UserMarkdownAboutMeUpdateEvent(id, value),

            nameof(UserIconUpdateEvent) when jObject["Icon"] is { } iconToken =>
                new UserIconUpdateEvent(id, iconToken.ToObject<Cid?>(serializer)),

            nameof(UserForgetMeUpdateEvent) when jObject["ForgetMe"] is { } forgetMeToken =>
                new UserForgetMeUpdateEvent(id, forgetMeToken.Value<bool?>()),

            nameof(ConnectionAddEvent) when jObject["Connection"] is { } connectionToken && connectionToken.ToObject<DagCid>(serializer) is { } value =>
                new ConnectionAddEvent(id, value),

            nameof(ConnectionRemoveEvent) when jObject["Connection"] is { } connectionToken && connectionToken.ToObject<DagCid>(serializer) is { } value =>
                new ConnectionRemoveEvent(id, value),

            nameof(UserLinkAddEvent) when jObject["Link"] is { } linkAddToken && linkAddToken.ToObject<Link>(serializer) is { } value =>
                new UserLinkAddEvent(id, value),

            nameof(UserLinkRemoveEvent) when jObject["Link"] is { } linkRemoveToken && linkRemoveToken.ToObject<Link>(serializer) is { } value =>
                new UserLinkRemoveEvent(id, value),

            nameof(UserProjectAddEvent) when jObject["Project"] is { } projectAddToken && projectAddToken.ToObject<Cid>(serializer) is { } value =>
                new UserProjectAddEvent(id, value),

            nameof(UserProjectRemoveEvent) when jObject["Project"] is { } projectRemoveToken && projectRemoveToken.ToObject<Cid>(serializer) is { } value =>
                new UserProjectRemoveEvent(id, value),

            nameof(UserPublisherAddEvent) when jObject["Publisher"] is { } publisherAddToken && publisherAddToken.ToObject<Cid>(serializer) is { } value =>
                new UserPublisherAddEvent(id, value),

            nameof(UserPublisherRemoveEvent) when jObject["Publisher"] is { } publisherRemoveToken && publisherRemoveToken.ToObject<Cid>(serializer) is { } value =>
                new UserPublisherRemoveEvent(id, value),

            _ => throw new InvalidOperationException($"Unhandled or missing event type: {eventId}")
        };
    }

    internal static WinAppCommunityUpdateEvent? ReadProject(JObject jObject, string eventId, string id, JsonSerializer serializer)
    {
        return eventId switch
        {
            nameof(ProjectNameUpdateEvent) when jObject["Name"] is { } nameToken && nameToken.Value<string>() is { } value =>
                new ProjectNameUpdateEvent(id, value),

            nameof(ProjectDescriptionUpdateEvent) when jObject["Description"] is { } descriptionToken && descriptionToken.Value<string>() is { } value =>
                new ProjectDescriptionUpdateEvent(id, value),

            nameof(ProjectIconUpdateEvent) when jObject["Icon"] is { } iconToken =>
                new ProjectIconUpdateEvent(id, iconToken.ToObject<Cid?>(serializer)),

            nameof(ProjectHeroImageUpdateEvent) when jObject["HeroImage"] is { } heroImageToken =>
                new ProjectHeroImageUpdateEvent(id, heroImageToken.ToObject<Cid?>(serializer)),

            nameof(ProjectImageAddEvent) when jObject["Image"] is { } imageAddToken && imageAddToken.ToObject<Cid>(serializer) is { } value =>
                new ProjectImageAddEvent(id, value),

            nameof(ProjectImageRemoveEvent) when jObject["Image"] is { } imageRemoveToken && imageRemoveToken.ToObject<Cid>(serializer) is { } value =>
                new ProjectImageRemoveEvent(id, value),

            nameof(ProjectFeatureAddEvent) when jObject["Feature"] is { } featureAddToken && featureAddToken.Value<string>() is { } value =>
                new ProjectFeatureAddEvent(id, value),

            nameof(ProjectFeatureRemoveEvent) when jObject["Feature"] is { } featureRemoveToken && featureRemoveToken.Value<string>() is { } value =>
                new ProjectFeatureRemoveEvent(id, value),

            nameof(ProjectLinkAddEvent) when jObject["Link"] is { } linkAddToken && linkAddToken.ToObject<Link>(serializer) is { } value =>
                new ProjectLinkAddEvent(id, value),

            nameof(ProjectLinkRemoveEvent) when jObject["Link"] is { } linkRemoveToken && linkRemoveToken.ToObject<Link>(serializer) is { } value =>
                new ProjectLinkRemoveEvent(id, value),

            nameof(ConnectionAddEvent) when jObject["Connection"] is { } connectionToken && connectionToken.ToObject<DagCid>(serializer) is { } value =>
                new ConnectionAddEvent(id, value),

            nameof(ConnectionRemoveEvent) when jObject["Connection"] is { } connectionToken && connectionToken.ToObject<DagCid>(serializer) is { } value =>
                new ConnectionRemoveEvent(id, value),

            nameof(ProjectForgetMeUpdateEvent) when jObject["ForgetMe"] is { } forgetMeToken =>
                new ProjectForgetMeUpdateEvent(id, forgetMeToken.Value<bool?>()),

            _ => throw new InvalidOperationException($"Unhandled or missing event type: {eventId}")
        };
    }

    internal static PublisherUpdateEvent? ReadPublisher(JObject jObject, string eventId, string id, JsonSerializer serializer)
    {
        return eventId switch
        {
            nameof(PublisherNameUpdateEvent) when jObject["Name"] is { } nameToken && nameToken.Value<string>() is { } value =>
                new PublisherNameUpdateEvent(id, value),

            nameof(PublisherDescriptionUpdateEvent) when jObject["Description"] is { } descriptionToken && descriptionToken.Value<string>() is { } value =>
                new PublisherDescriptionUpdateEvent(id, value),

            nameof(PublisherIconUpdateEvent) when jObject["Icon"] is { } iconToken =>
                new PublisherIconUpdateEvent(id, iconToken.ToObject<Cid?>(serializer)),

            nameof(PublisherOwnerUpdateEvent) when jObject["Owner"] is { } token && token.ToObject<Cid>(serializer) is { } value =>
                new PublisherOwnerUpdateEvent(id, value),

            nameof(PublisherAccentColorUpdateEvent) when jObject["AccentColor"] is { } accentColorToken =>
                new PublisherAccentColorUpdateEvent(id, accentColorToken.Value<string>()),

            nameof(PublisherLinkAddEvent) when jObject["Link"] is { } linkAddToken && linkAddToken.ToObject<Link>(serializer) is { } value =>
                new PublisherLinkAddEvent(id, value),

            nameof(PublisherLinkRemoveEvent) when jObject["Link"] is { } linkRemoveToken && linkRemoveToken.ToObject<Link>(serializer) is { } value =>
                new PublisherLinkRemoveEvent(id, value),

            nameof(PublisherProjectAddEvent) when jObject["Project"] is { } projectAddToken && projectAddToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherProjectAddEvent(id, value),

            nameof(PublisherProjectRemoveEvent) when jObject["Project"] is { } projectRemoveToken && projectRemoveToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherProjectRemoveEvent(id, value),

            nameof(PublisherUserAddEvent) when jObject["User"] is { } addToken && addToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherUserAddEvent(id, value),

            nameof(PublisherUserRemoveEvent) when jObject["User"] is { } removeToken && removeToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherUserRemoveEvent(id, value),

            nameof(PublisherParentPublisherAddEvent) when jObject["ParentPublishers"] is { } addToken && addToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherParentPublisherAddEvent(id, value),

            nameof(PublisherParentPublisherRemoveEvent) when jObject["ParentPublishers"] is { } removeToken && removeToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherParentPublisherRemoveEvent(id, value),

            nameof(PublisherChildPublisherAddEvent) when jObject["ChildPublishers"] is { } addToken && addToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherChildPublisherAddEvent(id, value),

            nameof(PublisherChildPublisherRemoveEvent) when jObject["ChildPublishers"] is { } removeToken && removeToken.ToObject<Cid>(serializer) is { } value =>
                new PublisherChildPublisherRemoveEvent(id, value),

            _ => throw new InvalidOperationException($"Unhandled or missing event type: {eventId}")
        };
    }
}
