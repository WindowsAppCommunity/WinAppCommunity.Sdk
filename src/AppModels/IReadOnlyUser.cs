using System;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Represents a user.
/// </summary>
public interface IReadOnlyUser
{
    public string Id { get; }
    
    /// <summary>
    /// The name of the user.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A summary of the user in markdown syntax.
    /// </summary>
    public string MarkdownAboutMe { get; }

    /// <summary>
    /// Represents application connections added by the user.
    /// </summary>
    public ApplicationConnection[] Connections { get; }

    /// <summary>
    /// Represents links to external profiles or resources added by the user.
    /// </summary>
    public Link[] Links { get; }

    /// <summary>
    /// A flag that indicates whether the profile has requested to be forgotten.
    /// </summary>
    public bool? ForgetMe { get; }
    
    /// <summary>
    /// Raised when <see cref="Name"/> is updated.
    /// </summary>
    public event EventHandler<string>? NameUpdated;
    
    /// <summary>
    /// Raised when <see cref="MarkdownAboutMe"/> is updated.
    /// </summary>
    public event EventHandler<string>? MarkdownAboutMeUpdated;
    
    /// <summary>
    /// Raised when <see cref="Connections"/> is updated.
    /// </summary>
    public event EventHandler<ApplicationConnection[]>? ConnectionsUpdated;
    
    /// <summary>
    /// Raised when <see cref="Links"/> is updated.
    /// </summary>
    public event EventHandler<Link[]>? LinksUpdated;

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