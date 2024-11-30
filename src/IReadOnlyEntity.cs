namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a read-only entity with common properties and events.
/// </summary>
public interface IReadOnlyEntity
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A description of the entity.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Represents links to external profiles or resources added by the entity.
    /// </summary>
    Link[] Links { get; }

    /// <summary>
    /// A flag that indicates whether the entity has requested to be forgotten.
    /// </summary>
    bool? ForgetMe { get; }

    /// <summary>
    /// Raised when <see cref="Name"/> is updated.
    /// </summary>
    event EventHandler<string>? NameUpdated;

    /// <summary>
    /// Raised when <see cref="Description"/> is updated.
    /// </summary>
    event EventHandler<string>? DescriptionUpdated;

    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    event EventHandler<Link[]>? LinksUpdated;

    /// <summary>
    /// Raised when <see cref="ForgetMe"/> is updated.
    /// </summary>
    event EventHandler<bool?>? ForgetMeUpdated;
}
