using Ipfs;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

/// <summary>
/// Represents an update event for a user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="EventId">The unique identifier of the event.</param>
public abstract record UserUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

/// <summary>
/// Represents an event where the user's name is updated.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The new name of the user.</param>
public record UserNameUpdateEvent(string Id, string Name) : UserUpdateEvent(Id, nameof(UserNameUpdateEvent));

/// <summary>
/// Represents an event where the user's "about me" section is updated with markdown.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="MarkdownAboutMe">The new markdown content for the "about me" section.</param>
public record UserMarkdownAboutMeUpdateEvent(string Id, string MarkdownAboutMe) : UserUpdateEvent(Id, nameof(UserMarkdownAboutMeUpdateEvent));

/// <summary>
/// Represents an event where the user's icon is updated.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Icon">The new icon of the user.</param>
public record UserIconUpdateEvent(string Id, Cid? Icon) : UserUpdateEvent(Id, nameof(UserIconUpdateEvent));

/// <summary>
/// Represents an event where the user's "forget me" status is updated.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="ForgetMe">The new "forget me" status of the user.</param>
public record UserForgetMeUpdateEvent(string Id, bool? ForgetMe) : UserUpdateEvent(Id, nameof(UserForgetMeUpdateEvent));

/// <summary>
/// Represents an event where a link is added to the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Link">The link to be added to the user.</param>
public record UserLinkAddEvent(string Id, Link Link) : UserUpdateEvent(Id, nameof(UserLinkAddEvent));

/// <summary>
/// Represents an event where a link is removed from the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Link">The link to be removed from the user.</param>
public record UserLinkRemoveEvent(string Id, Link Link) : UserUpdateEvent(Id, nameof(UserLinkRemoveEvent));

/// <summary>
/// Represents an event where a project is added to the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Project">The project to be added to the user.</param>
public record UserProjectAddEvent(string Id, Cid Project) : UserUpdateEvent(Id, nameof(UserProjectAddEvent));

/// <summary>
/// Represents an event where a project is removed from the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Project">The project to be removed from the user.</param>
public record UserProjectRemoveEvent(string Id, Cid Project) : UserUpdateEvent(Id, nameof(UserProjectRemoveEvent));

/// <summary>
/// Represents an event where a publisher is added to the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Publisher">The publisher to be added to the user.</param>
public record UserPublisherAddEvent(string Id, Cid Publisher) : UserUpdateEvent(Id, nameof(UserPublisherAddEvent));

/// <summary>
/// Represents an event where a publisher is removed from the user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Publisher">The publisher to be removed from the user.</param>
public record UserPublisherRemoveEvent(string Id, Cid Publisher) : UserUpdateEvent(Id, nameof(UserPublisherRemoveEvent));
