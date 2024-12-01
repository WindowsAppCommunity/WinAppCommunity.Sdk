using Ipfs;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

/// <summary>
/// Represents an update event for a project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="EventId">The unique identifier of the event.</param>
public abstract record ProjectUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

/// <summary>
/// Represents an event where the project's name is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Name">The new name of the project.</param>
public record ProjectNameUpdateEvent(string Id, string Name) : ProjectUpdateEvent(Id, nameof(ProjectNameUpdateEvent));

/// <summary>
/// Represents an event where the project's description is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Description">The new description of the project.</param>
public record ProjectDescriptionUpdateEvent(string Id, string Description) : ProjectUpdateEvent(Id, nameof(ProjectDescriptionUpdateEvent));

/// <summary>
/// Represents an event where the project's icon is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Icon">The new icon of the project.</param>
public record ProjectIconUpdateEvent(string Id, Cid? Icon) : ProjectUpdateEvent(Id, nameof(ProjectIconUpdateEvent));

/// <summary>
/// Represents an event where the project's hero image is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="HeroImage">The new hero image of the project.</param>
public record ProjectHeroImageUpdateEvent(string Id, Cid? HeroImage) : ProjectUpdateEvent(Id, nameof(ProjectHeroImageUpdateEvent));

/// <summary>
/// Represents an event where an image is added to the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Image">The image to be added to the project.</param>
public record ProjectImageAddEvent(string Id, Cid Image) : ProjectUpdateEvent(Id, nameof(ProjectImageAddEvent));

/// <summary>
/// Represents an event where an image is removed from the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Image">The image to be removed from the project.</param>
public record ProjectImageRemoveEvent(string Id, Cid Image) : ProjectUpdateEvent(Id, nameof(ProjectImageRemoveEvent));

/// <summary>
/// Represents an event where a feature is added to the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Feature">The feature to be added to the project.</param>
public record ProjectFeatureAddEvent(string Id, string Feature) : ProjectUpdateEvent(Id, nameof(ProjectFeatureAddEvent));

/// <summary>
/// Represents an event where a feature is removed from the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Feature">The feature to be removed from the project.</param>
public record ProjectFeatureRemoveEvent(string Id, string Feature) : ProjectUpdateEvent(Id, nameof(ProjectFeatureRemoveEvent));

/// <summary>
/// Represents an event where the project's accent color is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="AccentColor">The new accent color of the project.</param>
public record ProjectAccentColorUpdateEvent(string Id, string? AccentColor) : ProjectUpdateEvent(Id, nameof(ProjectAccentColorUpdateEvent));

/// <summary>
/// Represents an event where the project's category is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Category">The new category of the project.</param>
public record ProjectCategoryUpdateEvent(string Id, string Category) : ProjectUpdateEvent(Id, nameof(ProjectCategoryUpdateEvent));

/// <summary>
/// Represents an event where a dependency is added to the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Dependency">The dependency to be added to the project.</param>
public record ProjectDependencyAddEvent(string Id, Cid Dependency) : ProjectUpdateEvent(Id, nameof(ProjectDependencyAddEvent));

/// <summary>
/// Represents an event where a dependency is removed from the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Dependency">The dependency to be removed from the project.</param>
public record ProjectDependencyRemoveEvent(string Id, Cid Dependency) : ProjectUpdateEvent(Id, nameof(ProjectDependencyRemoveEvent));

/// <summary>
/// Represents an event where a link is added to the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Link">The link to be added to the project.</param>
public record ProjectLinkAddEvent(string Id, Link Link) : ProjectUpdateEvent(Id, nameof(ProjectLinkAddEvent));

/// <summary>
/// Represents an event where a link is removed from the project.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="Link">The link to be removed from the project.</param>
public record ProjectLinkRemoveEvent(string Id, Link Link) : ProjectUpdateEvent(Id, nameof(ProjectLinkRemoveEvent));

/// <summary>
/// Represents an event where the project's "forget me" status is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="ForgetMe">The new "forget me" status of the project.</param>
public record ProjectForgetMeUpdateEvent(string Id, bool? ForgetMe) : ProjectUpdateEvent(Id, nameof(ProjectForgetMeUpdateEvent));

/// <summary>
/// Represents an event where the project's privacy status is updated.
/// </summary>
/// <param name="Id">The unique identifier of the project.</param>
/// <param name="IsUnlisted">The new privacy status of the project.</param>
public record ProjectPrivacyUpdateEvent(string Id, bool IsUnlisted) : ProjectUpdateEvent(Id, nameof(ProjectPrivacyUpdateEvent));
