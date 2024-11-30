using Ipfs;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

public abstract record UserUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

public record UserNameUpdateEvent(string Id, string Name) : UserUpdateEvent(Id, nameof(UserNameUpdateEvent));

public record UserMarkdownAboutMeUpdateEvent(string Id, string MarkdownAboutMe) : UserUpdateEvent(Id, nameof(UserMarkdownAboutMeUpdateEvent));

public record UserIconUpdateEvent(string Id, Cid? Icon) : UserUpdateEvent(Id, nameof(UserIconUpdateEvent));

public record UserForgetMeUpdateEvent(string Id, bool? ForgetMe) : UserUpdateEvent(Id, nameof(UserForgetMeUpdateEvent));

public record UserLinkAddEvent(string Id, Link Link) : UserUpdateEvent(Id, nameof(UserLinkAddEvent));

public record UserLinkRemoveEvent(string Id, Link Link) : UserUpdateEvent(Id, nameof(UserLinkRemoveEvent));

public record UserProjectAddEvent(string Id, Cid Project) : UserUpdateEvent(Id, nameof(UserProjectAddEvent));

public record UserProjectRemoveEvent(string Id, Cid Project) : UserUpdateEvent(Id, nameof(UserProjectRemoveEvent));

public record UserPublisherAddEvent(string Id, Cid Publisher) : UserUpdateEvent(Id, nameof(UserPublisherAddEvent));

public record UserPublisherRemoveEvent(string Id, Cid Publisher) : UserUpdateEvent(Id, nameof(UserPublisherRemoveEvent));
