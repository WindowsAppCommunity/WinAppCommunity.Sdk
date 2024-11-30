namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a user.
/// </summary>
public interface IReadOnlyUser : IReadOnlyEntity, IReadOnlyPublisherRoleCollection, IReadOnlyProjectRoleCollection
{
}
