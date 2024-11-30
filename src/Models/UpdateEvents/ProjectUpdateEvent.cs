using Ipfs;

namespace WinAppCommunity.Sdk.Models.UpdateEvents;

public abstract record ProjectUpdateEvent(string Id, string EventId) : WinAppCommunityUpdateEvent(Id, EventId);

public record ProjectNameUpdateEvent(string Id, string Name) : ProjectUpdateEvent(Id, nameof(ProjectNameUpdateEvent));

public record ProjectDescriptionUpdateEvent(string Id, string Description) : ProjectUpdateEvent(Id, nameof(ProjectDescriptionUpdateEvent));

public record ProjectIconUpdateEvent(string Id, Cid? Icon) : ProjectUpdateEvent(Id, nameof(ProjectIconUpdateEvent));

public record ProjectHeroImageUpdateEvent(string Id, Cid? HeroImage) : ProjectUpdateEvent(Id, nameof(ProjectHeroImageUpdateEvent));

public record ProjectImageAddEvent(string Id, Cid Image) : ProjectUpdateEvent(Id, nameof(ProjectImageAddEvent));

public record ProjectImageRemoveEvent(string Id, Cid Image) : ProjectUpdateEvent(Id, nameof(ProjectImageRemoveEvent));

public record ProjectFeatureAddEvent(string Id, string Feature) : ProjectUpdateEvent(Id, nameof(ProjectFeatureAddEvent));

public record ProjectFeatureRemoveEvent(string Id, string Feature) : ProjectUpdateEvent(Id, nameof(ProjectFeatureRemoveEvent));

public record ProjectAccentColorUpdateEvent(string Id, string? AccentColor) : ProjectUpdateEvent(Id, nameof(ProjectAccentColorUpdateEvent));

public record ProjectCategoryUpdateEvent(string Id, string Category) : ProjectUpdateEvent(Id, nameof(ProjectCategoryUpdateEvent));

public record ProjectDependencyAddEvent(string Id, Cid Dependency) : ProjectUpdateEvent(Id, nameof(ProjectDependencyAddEvent));

public record ProjectDependencyRemoveEvent(string Id, Cid Dependency) : ProjectUpdateEvent(Id, nameof(ProjectDependencyRemoveEvent));

public record ProjectLinkAddEvent(string Id, Link Link) : ProjectUpdateEvent(Id, nameof(ProjectLinkAddEvent));

public record ProjectLinkRemoveEvent(string Id, Link Link) : ProjectUpdateEvent(Id, nameof(ProjectLinkRemoveEvent));

public record ProjectForgetMeUpdateEvent(string Id, bool? ForgetMe) : ProjectUpdateEvent(Id, nameof(ProjectForgetMeUpdateEvent));

public record ProjectPrivacyUpdateEvent(string Id, bool IsUnlisted) : ProjectUpdateEvent(Id, nameof(ProjectPrivacyUpdateEvent));
