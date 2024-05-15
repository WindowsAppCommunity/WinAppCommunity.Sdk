using CommunityToolkit.Diagnostics;
using Ipfs;
using OwlCore.ComponentModel.Nomad;
using OwlCore.Kubo;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.AppModels;

/// <summary>
/// Creates a new instance of <see cref="ModifiablePublisherAppModel"/>.
/// </summary>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{T}.TryAdvanceEventStreamAsync"/>. </param>
public class ModifiablePublisherAppModel(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ModifiablePublisherNomadKuboEventStreamHandler(listeningEventStreamHandlers)
{
    /// <summary>
    /// Gets the icon file for this user.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async Task<IFile?> GetIconFileAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var iconCid = Inner.Icon;
        if (iconCid is null)
            return null;

        return new IpfsFile(iconCid, $"{nameof(User)}.{Id}.png", Client);
    }

    /// <summary>
    /// Get the projects for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ModifiableProjectAppModel> GetProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var projectCid in Inner.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Project>(projectCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ModifiableProjectAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                LocalEventStreamKeyName = LocalEventStreamKeyName,
                Id = projectCid, // assuming project cid is ipns and won't change
                IpnsLifetime = IpnsLifetime,
                Sources = Sources,
                UseCache = UseCache,
                ShouldPin = ShouldPin,
                Inner = result,
            };
        }
    }

    /// <summary>
    /// Get the publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ModifiablePublisherAppModel> GetChildPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherCid in Inner.ChildPublishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ModifiablePublisherAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                LocalEventStreamKeyName = LocalEventStreamKeyName,
                Id = publisherCid, // assuming publisher cid is ipns and won't change
                IpnsLifetime = IpnsLifetime,
                Sources = Sources,
                UseCache = UseCache,
                ShouldPin = ShouldPin,
                Inner = result,
            };
        }
    }

    /// <summary>
    /// Get the publishers for this publisher.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing operation.</param>
    public async IAsyncEnumerable<ModifiablePublisherAppModel> GetParentPublishersAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var publisherCid in Inner.ParentPublishers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (result, _) = await Client.ResolveDagCidAsync<Publisher>(publisherCid, nocache: !UseCache, cancellationToken);
            Guard.IsNotNull(result);

            yield return new ModifiablePublisherAppModel(ListeningEventStreamHandlers)
            {
                Client = Client,
                LocalEventStreamKeyName = LocalEventStreamKeyName,
                Id = publisherCid, // assuming publisher cid is ipns and won't change
                IpnsLifetime = IpnsLifetime,
                Sources = Sources,
                UseCache = UseCache,
                ShouldPin = ShouldPin,
                Inner = result,
            };
        }
    }

    public async Task UpdateNameAsync(string newName, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherNameUpdateEvent(Id, newName);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateDescriptionAsync(string newDescription, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherDescriptionUpdateEvent(Id, newDescription);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateIconAsync(Cid? newIcon, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherIconUpdateEvent(Id, newIcon);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateAccentColorAsync(string? newAccentColor, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherAccentColorUpdateEvent(Id, newAccentColor);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task UpdateContactEmailAsync(EmailConnection? newContactEmail, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherContactEmailUpdateEvent(Id, newContactEmail);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }

    public async Task AddLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherLinkAddEvent(Id, link);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    public async Task RemoveLinkAsync(Link link, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherLinkRemoveEvent(Id, link);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    public async Task AddProjectAsync(Cid publisher, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherProjectAddEvent(Id, publisher);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    public async Task RemoveProjectAsync(Cid publisher, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherProjectRemoveEvent(Id, publisher);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    public async Task AddChildPublisherAsync(Cid childPublisher, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherChildPublisherAddEvent(Id, childPublisher);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    public async Task RemoveChildPublisherAsync(Cid childPublisher, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherChildPublisherRemoveEvent(Id, childPublisher);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    public async Task AddParentPublisherAsync(Cid parentPublisher, CancellationToken cancellationToken)
    {
        var addEvent = new PublisherParentPublisherAddEvent(Id, parentPublisher);
        await ApplyEntryUpdateAsync(addEvent, cancellationToken);
        await AppendNewEntryAsync(addEvent, cancellationToken);
    }

    public async Task RemoveParentPublisherAsync(Cid parentPublisher, CancellationToken cancellationToken)
    {
        var removeEvent = new PublisherParentPublisherRemoveEvent(Id, parentPublisher);
        await ApplyEntryUpdateAsync(removeEvent, cancellationToken);
        await AppendNewEntryAsync(removeEvent, cancellationToken);
    }

    public async Task UpdatePrivateFlagAsync(bool isPrivate, CancellationToken cancellationToken)
    {
        var updateEvent = new PublisherPrivateFlagUpdateEvent(Id, isPrivate);
        await ApplyEntryUpdateAsync(updateEvent, cancellationToken);
        await AppendNewEntryAsync(updateEvent, cancellationToken);
    }
}