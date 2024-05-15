using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.ComponentModel.Nomad;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.Nomad;

/// <summary>
/// A read-only Nomad event stream handler for users.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ReadOnlyUserNomadKuboEventStreamHandler"/>.
/// </remarks>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{T}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyUserNomadKuboEventStreamHandler(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyNomadKuboEventStreamHandler<UserUpdateEvent>(listeningEventStreamHandlers), IDelegable<Models.User>
{
    /// <summary>
    /// The inner <see cref="Models.User"/> record to alter when handling the event stream.
    /// </summary>
    public required Models.User Inner { get; set; }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Name = string.Empty;
        Inner.Icon = null;
        Inner.MarkdownAboutMe = string.Empty;
        Inner.Connections = [];
        Inner.Links = [];
        Inner.Projects = [];
        Inner.Publishers = [];
        Inner.ForgetMe = false;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task ApplyEntryUpdateAsync(UserUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (updateEvent is UserNameUpdateEvent userNameUpdate)
            Inner.Name = userNameUpdate.Name;

        if (updateEvent is UserMarkdownAboutMeUpdateEvent markdownAboutMeUpdate)
            Inner.MarkdownAboutMe = markdownAboutMeUpdate.MarkdownAboutMe;

        if (updateEvent is UserIconUpdateEvent userIconUpdate)
            Inner.Icon = userIconUpdate.Icon;

        if (updateEvent is UserForgetMeUpdateEvent forgetMeUpdate)
            Inner.ForgetMe = forgetMeUpdate.ForgetMe;

        if (updateEvent is UserConnectionAddEvent connectionAdd)
            Inner.Connections = Inner.Connections.Append(connectionAdd.Connection).ToArray();

        if (updateEvent is UserConnectionRemoveEvent connectionRemove)
            Inner.Connections = Inner.Connections.Where(p => p != connectionRemove.Connection).ToArray();

        if (updateEvent is UserLinkAddEvent linkAdd)
            Inner.Links = Inner.Links.Append(linkAdd.Link).ToArray();

        if (updateEvent is UserLinkRemoveEvent linkRemove)
            Inner.Links = Inner.Links.Where(l => l != linkRemove.Link).ToArray();

        if (updateEvent is UserProjectAddEvent projectAdd)
            Inner.Projects = Inner.Projects.Append(projectAdd.Project).ToArray();

        if (updateEvent is UserProjectRemoveEvent projectRemove)
            Inner.Projects = Inner.Projects.Where(p => p != projectRemove.Project).ToArray();

        if (updateEvent is UserPublisherAddEvent publisherAdd)
            Inner.Publishers = Inner.Publishers.Append(publisherAdd.Publisher).ToArray();

        if (updateEvent is UserPublisherRemoveEvent publisherRemove)
            Inner.Publishers = Inner.Publishers.Where(p => p != publisherRemove.Publisher).ToArray();

        return Task.CompletedTask;
    }
}