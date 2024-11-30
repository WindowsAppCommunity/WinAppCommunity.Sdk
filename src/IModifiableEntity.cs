namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a modifiable entity with common methods.
/// </summary>
public interface IModifiableEntity : IReadOnlyEntity
{
    /// <summary>
    /// Updates the name of this entity.
    /// </summary>
    public Task UpdateNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the description of this entity.
    /// </summary>
    public Task UpdateDescriptionAsync(string description, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the forget-me status of this entity.
    /// </summary>
    public Task UpdateForgetMeStatusAsync(bool? forgetMe, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a link to this entity.
    /// </summary>
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a link from this entity.
    /// </summary>
    public Task RemoveLinkAsync(Link link, CancellationToken cancellationToken);
}
