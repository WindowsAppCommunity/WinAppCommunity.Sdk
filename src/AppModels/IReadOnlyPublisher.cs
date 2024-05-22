using OwlCore.ComponentModel;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WinAppCommunity.Sdk.AppModels;

public interface IReadOnlyPublisher : IDelegable<Models.Publisher>
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
    /// Get the child publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyPublisher> GetChildPublishersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the parent publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyPublisher> GetParentPublishersAsync(CancellationToken cancellationToken);
}