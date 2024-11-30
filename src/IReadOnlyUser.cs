using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ipfs;
using OwlCore.Storage;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk;

/// <summary>
/// Represents a user.
/// </summary>
public interface IReadOnlyUser : IReadOnlyEntity
{
    public string Id { get; }
    
    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public Dictionary<string, DagCid> Connections { get; }

    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the projects for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<(IReadOnlyProject Project, Role Role)> GetProjectsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the publishers for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public IAsyncEnumerable<(IReadOnlyPublisher Publisher, Role Role)> GetPublishersAsync(CancellationToken cancellationToken);
}
