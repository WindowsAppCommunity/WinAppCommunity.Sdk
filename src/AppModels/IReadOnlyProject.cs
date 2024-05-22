using OwlCore.ComponentModel;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

public interface IReadOnlyProject : IDelegable<Project>
{
    /// <summary>
    /// Gets the image files for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IFile> GetImageFilesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a file containing the hero image for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetHeroImageFileAsync(CancellationToken cancellationToken);

    public IAsyncEnumerable<(IReadOnlyUser User, Role Role)> GetCollaboratorsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the publisher for this project.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IReadOnlyPublisher> GetPublisherAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the dependencies for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<IReadOnlyProject> GetDependenciesAsync(CancellationToken cancellationToken);
}