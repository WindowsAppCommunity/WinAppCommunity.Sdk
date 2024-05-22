using Ipfs;
using OwlCore.ComponentModel;
using OwlCore.Nomad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinAppCommunity.Sdk.Models;
using WinAppCommunity.Sdk.Nomad.Kubo;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.Nomad;

/// <summary>
/// A read-only Nomad event stream handler for projects.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ReadOnlyProjectNomadKuboEventStreamHandler"/>.
/// </remarks>
/// <param name="listeningEventStreamHandlers">A shared collection of all available event streams that should participate in playback of events using their respective <see cref="IEventStreamHandler{TEventStreamEntry}.TryAdvanceEventStreamAsync"/>. </param>
public class ReadOnlyProjectNomadKuboEventStreamHandler(ICollection<ISharedEventStreamHandler<Cid, KuboNomadEventStream, KuboNomadEventStreamEntry>> listeningEventStreamHandlers)
    : ReadOnlyNomadKuboEventStreamHandler<ProjectUpdateEvent>(listeningEventStreamHandlers), IDelegable<Project>
{
    /// <summary>
    /// The inner <see cref="Project"/> record to alter when handling the event stream.
    /// </summary>
    public required Project Inner { get; set; }

    /// <inheritdoc />
    public override Task ResetEventStreamPositionAsync(CancellationToken cancellationToken)
    {
        Inner.Name = string.Empty;
        Inner.Description = string.Empty;
        Inner.Icon = null;
        Inner.HeroImage = null;
        Inner.Images = Array.Empty<Cid>();
        Inner.Features = Array.Empty<string>();
        Inner.AccentColor = null;
        Inner.Category = string.Empty;
        Inner.Dependencies = Array.Empty<Cid>();
        Inner.Collaborators = Array.Empty<Collaborator>();
        Inner.Links = Array.Empty<Link>();
        Inner.PublishedProjectConnections = Array.Empty<ApplicationConnection>();
        Inner.ForgetMe = false;
        Inner.IsPrivate = false;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task ApplyEntryUpdateAsync(ProjectUpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        if (updateEvent is ProjectNameUpdateEvent projectNameUpdate)
            Inner.Name = projectNameUpdate.Name;

        if (updateEvent is ProjectDescriptionUpdateEvent descriptionUpdate)
            Inner.Description = descriptionUpdate.Description;

        if (updateEvent is ProjectIconUpdateEvent iconUpdate)
            Inner.Icon = iconUpdate.Icon;

        if (updateEvent is ProjectHeroImageUpdateEvent heroImageUpdate)
            Inner.HeroImage = heroImageUpdate.HeroImage;

        if (updateEvent is ProjectFeatureAddEvent featureAddEvent)
            Inner.Features = Inner.Features.Append(featureAddEvent.Feature).ToArray();

        if (updateEvent is ProjectFeatureRemoveEvent featureRemoveEvent)
            Inner.Features = Inner.Features.Where(f => f != featureRemoveEvent.Feature).ToArray();

        if (updateEvent is ProjectImageAddEvent imageAddEvent)
            Inner.Images = Inner.Images.Append(imageAddEvent.Image).ToArray();

        if (updateEvent is ProjectImageRemoveEvent imageRemoveEvent)
            Inner.Images = Inner.Images.Where(img => img != imageRemoveEvent.Image).ToArray();

        if (updateEvent is ProjectDependencyAddEvent dependencyAddEvent)
            Inner.Dependencies = Inner.Dependencies.Append(dependencyAddEvent.Dependency).ToArray();

        if (updateEvent is ProjectDependencyRemoveEvent dependencyRemoveEvent)
            Inner.Dependencies = Inner.Dependencies.Where(dep => dep != dependencyRemoveEvent.Dependency).ToArray();

        if (updateEvent is ProjectCollaboratorAddEvent collaboratorAddEvent)
            Inner.Collaborators = Inner.Collaborators.Append(collaboratorAddEvent.Collaborator).ToArray();

        if (updateEvent is ProjectCollaboratorRemoveEvent collaboratorRemoveEvent)
            Inner.Collaborators = Inner.Collaborators.Where(collab => collab != collaboratorRemoveEvent.Collaborator).ToArray();

        if (updateEvent is ProjectLinkAddEvent linkAddEvent)
            Inner.Links = Inner.Links.Append(linkAddEvent.Link).ToArray();

        if (updateEvent is ProjectLinkRemoveEvent linkRemoveEvent)
            Inner.Links = Inner.Links.Where(link => link != linkRemoveEvent.Link).ToArray();

        if (updateEvent is ProjectPublishedConnectionAddEvent connectionAddEvent)
            Inner.PublishedProjectConnections = Inner.PublishedProjectConnections.Append(connectionAddEvent.Connection).ToArray();

        if (updateEvent is ProjectPublishedConnectionRemoveEvent connectionRemoveEvent)
            Inner.PublishedProjectConnections = Inner.PublishedProjectConnections.Where(conn => conn != connectionRemoveEvent.Connection).ToArray();

        if (updateEvent is ProjectAccentColorUpdateEvent accentColorUpdate)
            Inner.AccentColor = accentColorUpdate.AccentColor;

        if (updateEvent is ProjectCategoryUpdateEvent categoryUpdate)
            Inner.Category = categoryUpdate.Category;

        if (updateEvent is ProjectForgetMeUpdateEvent forgetMeUpdate)
            Inner.ForgetMe = forgetMeUpdate.ForgetMe;

        if (updateEvent is ProjectPrivacyUpdateEvent privacyUpdate)
            Inner.IsPrivate = privacyUpdate.IsPrivate;

        return Task.CompletedTask;
    }

}