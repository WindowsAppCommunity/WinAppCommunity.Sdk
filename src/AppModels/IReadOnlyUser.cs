using OwlCore.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a user.
/// </summary>
public interface IReadOnlyUser : IDelegable<Models.User>
{
    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the projects for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyProject> GetProjectsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the publishers for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyPublisher> GetPublishersAsync(CancellationToken cancellationToken);
}