using Ipfs;
using OwlCore.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Nomad;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.Nomad;

/// <summary>
/// A read-only shared stream handler implementation for publisher events.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ReadOnlyPublisherNomadKuboEventStreamHandler"/>.
/// </remarks>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>.</param>
public abstract class ReadOnlyPublisherNomadKuboEventStreamHandler(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyNomadKuboEventStreamHandler<PublisherUpdateEvent>(listeningEventStreamHandlers), IDelegable<Publisher>
{
    /// <summary>
    /// The inner <see cref="Publisher"/> record to alter when handling the event stream.
    /// </summary>
    public Publisher Inner { get; set; } = new();

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Name = string.Empty;
        Inner.Description = string.Empty;
        Inner.Icon = null;
        Inner.AccentColor = null;
        Inner.ContactEmail = null;
        Inner.Links = [];
        Inner.Projects = [];
        Inner.Users = [];
        Inner.ChildPublishers = [];
        Inner.IsPrivate = false;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task ApplyEntryUpdateAsync(PublisherUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (updateEvent is PublisherNameUpdateEvent nameUpdate)
            Inner.Name = nameUpdate.Name;

        if (updateEvent is PublisherDescriptionUpdateEvent descriptionUpdate)
            Inner.Description = descriptionUpdate.Description;

        if (updateEvent is PublisherIconUpdateEvent iconUpdate)
            Inner.Icon = iconUpdate.Icon;

        if (updateEvent is PublisherAccentColorUpdateEvent accentColorUpdate)
            Inner.AccentColor = accentColorUpdate.AccentColor;

        if (updateEvent is PublisherContactEmailUpdateEvent contactEmailUpdate)
            Inner.ContactEmail = contactEmailUpdate.ContactEmail;

        if (updateEvent is PublisherLinkAddEvent linkAdd)
            Inner.Links = Inner.Links.Append(linkAdd.Link).ToArray();

        if (updateEvent is PublisherLinkRemoveEvent linkRemove)
            Inner.Links = Inner.Links.Where(l => l != linkRemove.Link).ToArray();

        if (updateEvent is PublisherProjectAddEvent projectAdd)
            Inner.Projects = Inner.Projects.Append(projectAdd.Project).ToArray();

        if (updateEvent is PublisherProjectRemoveEvent projectRemove)
            Inner.Projects = Inner.Projects.Where(p => p != projectRemove.Project).ToArray();

        if (updateEvent is PublisherChildPublisherAddEvent childPublisherAdd)
            Inner.ChildPublishers = Inner.ChildPublishers.Append(childPublisherAdd.ChildPublisher).ToArray();

        if (updateEvent is PublisherChildPublisherRemoveEvent childPublisherRemove)
            Inner.ChildPublishers = Inner.ChildPublishers.Where(p => p != childPublisherRemove.ChildPublisher).ToArray();

        if (updateEvent is PublisherParentPublisherAddEvent parentPublisherAdd)
            Inner.ParentPublishers = Inner.ParentPublishers.Append(parentPublisherAdd.ParentPublisher).ToArray();

        if (updateEvent is PublisherParentPublisherRemoveEvent parentPublisherRemove)
            Inner.ParentPublishers = Inner.ParentPublishers.Where(p => p != parentPublisherRemove.ParentPublisher).ToArray();

        if (updateEvent is PublisherPrivateFlagUpdateEvent isPrivateUpdate)
            Inner.IsPrivate = isPrivateUpdate.IsPrivate;

        return Task.CompletedTask;
    }
}