using Ipfs;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

/// <summary>
/// Represents an update event for a publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="EventId">The unique identifier of the event.</param>
public abstract record PublisherUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

/// <summary>
/// Represents an event where the publisher's name is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Name">The new name of the publisher.</param>
public record PublisherNameUpdateEvent(string Id, string Name) : PublisherUpdateEvent(Id, nameof(PublisherNameUpdateEvent));

/// <summary>
/// Represents an event where the publisher's description is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Description">The new description of the publisher.</param>
public record PublisherDescriptionUpdateEvent(string Id, string Description) : PublisherUpdateEvent(Id, nameof(PublisherDescriptionUpdateEvent));

/// <summary>
/// Represents an event where the publisher's icon is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Icon">The new icon of the publisher.</param>
public record PublisherIconUpdateEvent(string Id, Cid? Icon) : PublisherUpdateEvent(Id, nameof(PublisherIconUpdateEvent));

/// <summary>
/// Represents an event where the publisher's owner is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Owner">The new owner of the publisher.</param>
public record PublisherOwnerUpdateEvent(string Id, Cid Owner) : PublisherUpdateEvent(Id, nameof(PublisherOwnerUpdateEvent));

/// <summary>
/// Represents an event where the publisher's accent color is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="AccentColor">The new accent color of the publisher.</param>
public record PublisherAccentColorUpdateEvent(string Id, string? AccentColor) : PublisherUpdateEvent(Id, nameof(PublisherAccentColorUpdateEvent));

/// <summary>
/// Represents an event where a link is added to the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Link">The link to be added to the publisher.</param>
public record PublisherLinkAddEvent(string Id, Link Link) : PublisherUpdateEvent(Id, nameof(PublisherLinkAddEvent));

/// <summary>
/// Represents an event where a link is removed from the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Link">The link to be removed from the publisher.</param>
public record PublisherLinkRemoveEvent(string Id, Link Link) : PublisherUpdateEvent(Id, nameof(PublisherLinkRemoveEvent));

/// <summary>
/// Represents an event where a project is added to the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Project">The project to be added to the publisher.</param>
public record PublisherProjectAddEvent(string Id, Cid Project) : PublisherUpdateEvent(Id, nameof(PublisherProjectAddEvent));

/// <summary>
/// Represents an event where a project is removed from the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="Project">The project to be removed from the publisher.</param>
public record PublisherProjectRemoveEvent(string Id, Cid Project) : PublisherUpdateEvent(Id, nameof(PublisherProjectRemoveEvent));

/// <summary>
/// Represents an event where a user is added to the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="User">The user to be added to the publisher.</param>
public record PublisherUserAddEvent(string Id, Cid User) : PublisherUpdateEvent(Id, nameof(PublisherUserAddEvent));

/// <summary>
/// Represents an event where a user is removed from the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="User">The user to be removed from the publisher.</param>
public record PublisherUserRemoveEvent(string Id, Cid User) : PublisherUpdateEvent(Id, nameof(PublisherUserRemoveEvent));

/// <summary>
/// Represents an event where a child publisher is added to the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="ChildPublisher">The child publisher to be added.</param>
public record PublisherChildPublisherAddEvent(string Id, Cid ChildPublisher) : PublisherUpdateEvent(Id, nameof(PublisherChildPublisherAddEvent));

/// <summary>
/// Represents an event where a child publisher is removed from the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="ChildPublisher">The child publisher to be removed.</param>
public record PublisherChildPublisherRemoveEvent(string Id, Cid ChildPublisher) : PublisherUpdateEvent(Id, nameof(PublisherChildPublisherRemoveEvent));

/// <summary>
/// Represents an event where a parent publisher is added to the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="ParentPublisher">The parent publisher to be added.</param>
public record PublisherParentPublisherAddEvent(string Id, Cid ParentPublisher) : PublisherUpdateEvent(Id, nameof(PublisherParentPublisherAddEvent));

/// <summary>
/// Represents an event where a parent publisher is removed from the publisher.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="ParentPublisher">The parent publisher to be removed.</param>
public record PublisherParentPublisherRemoveEvent(string Id, Cid ParentPublisher) : PublisherUpdateEvent(Id, nameof(PublisherParentPublisherRemoveEvent));

/// <summary>
/// Represents an event where the publisher's private flag is updated.
/// </summary>
/// <param name="Id">The unique identifier of the publisher.</param>
/// <param name="IsUnlisted">The new private flag status of the publisher.</param>
public record PublisherPrivateFlagUpdateEvent(string Id, bool IsUnlisted) : PublisherUpdateEvent(Id, nameof(PublisherPrivateFlagUpdateEvent));
