using Ipfs;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.Nomad.UpdateEvents;

public abstract record PublisherUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

public record PublisherNameUpdateEvent(string Id, string Name) : PublisherUpdateEvent(Id, nameof(PublisherNameUpdateEvent));

public record PublisherDescriptionUpdateEvent(string Id, string Description) : PublisherUpdateEvent(Id, nameof(PublisherDescriptionUpdateEvent));

public record PublisherIconUpdateEvent(string Id, Cid? Icon) : PublisherUpdateEvent(Id, nameof(PublisherIconUpdateEvent));

public record PublisherOwnerUpdateEvent(string Id, Cid Owner) : PublisherUpdateEvent(Id, nameof(PublisherOwnerUpdateEvent));

public record PublisherAccentColorUpdateEvent(string Id, string? AccentColor) : PublisherUpdateEvent(Id, nameof(PublisherAccentColorUpdateEvent));

public record PublisherContactEmailUpdateEvent(string Id, EmailConnection? ContactEmail) : PublisherUpdateEvent(Id, nameof(PublisherContactEmailUpdateEvent));

public record PublisherLinkAddEvent(string Id, Link Link) : PublisherUpdateEvent(Id, nameof(PublisherLinkAddEvent));

public record PublisherLinkRemoveEvent(string Id, Link Link) : PublisherUpdateEvent(Id, nameof(PublisherLinkRemoveEvent));

public record PublisherProjectAddEvent(string Id, Cid Project) : PublisherUpdateEvent(Id, nameof(PublisherProjectAddEvent));

public record PublisherProjectRemoveEvent(string Id, Cid Project) : PublisherUpdateEvent(Id, nameof(PublisherProjectRemoveEvent));

public record PublisherUserAddEvent(string Id, Cid User) : PublisherUpdateEvent(Id, nameof(PublisherUserAddEvent));

public record PublisherUserRemoveEvent(string Id, Cid User) : PublisherUpdateEvent(Id, nameof(PublisherUserRemoveEvent));

public record PublisherChildPublisherAddEvent(string Id, Cid ChildPublisher) : PublisherUpdateEvent(Id, nameof(PublisherChildPublisherAddEvent));

public record PublisherChildPublisherRemoveEvent(string Id, Cid ChildPublisher) : PublisherUpdateEvent(Id, nameof(PublisherChildPublisherRemoveEvent));

public record PublisherParentPublisherAddEvent(string Id, Cid ParentPublisher) : PublisherUpdateEvent(Id, nameof(PublisherParentPublisherAddEvent));

public record PublisherParentPublisherRemoveEvent(string Id, Cid ParentPublisher) : PublisherUpdateEvent(Id, nameof(PublisherParentPublisherRemoveEvent));

public record PublisherPrivateFlagUpdateEvent(string Id, bool IsPrivate) : PublisherUpdateEvent(Id, nameof(PublisherPrivateFlagUpdateEvent));
