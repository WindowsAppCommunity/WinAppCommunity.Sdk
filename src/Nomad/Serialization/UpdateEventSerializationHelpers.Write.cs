using Newtonsoft.Json.Linq;
using WinAppCommunity.Sdk.Nomad.UpdateEvents;

namespace WinAppCommunity.Sdk.AppModels;

internal static partial class UpdateEventSerializationHelpers
{
    internal static JObject? Write(WinAppCommunityUpdateEvent @event)
    {
        var jObject = new JObject
        {
            { "Id", new JValue(@event.Id) },
            { "eventId", new JValue(@event.EventId) }
        };

        WriteUser(@event, jObject);
        WriteProject(@event, jObject);
        WritePublisher(@event, jObject);

        return jObject;
    }

    private static void WriteUser(WinAppCommunityUpdateEvent @event, JObject jObject)
    {
        switch (@event)
        {
            case UserNameUpdateEvent userNameUpdateEvent:
                jObject.Add("Name", new JValue(userNameUpdateEvent.Name));
                break;
            case UserMarkdownAboutMeUpdateEvent markdownAboutMeUpdateEvent:
                jObject.Add("MarkdownAboutMe", new JValue(markdownAboutMeUpdateEvent.MarkdownAboutMe));
                break;
            case UserIconUpdateEvent iconUpdateEvent:
                jObject.Add("Icon",
                    iconUpdateEvent.Icon == null ? null : JValue.CreateString(iconUpdateEvent.Icon.ToString()));
                break;
            case UserForgetMeUpdateEvent forgetMeUpdateEvent:
                jObject.Add("ForgetMe",
                    forgetMeUpdateEvent.ForgetMe == null ? null : new JValue(forgetMeUpdateEvent.ForgetMe.Value));
                break;
            case UserConnectionAddEvent connectionAddEvent:
                jObject.Add("Connection", JObject.FromObject(connectionAddEvent.Connection));
                break;
            case UserConnectionRemoveEvent connectionRemoveEvent:
                jObject.Add("Connection", JObject.FromObject(connectionRemoveEvent.Connection));
                break;
            case UserLinkAddEvent linkAddEvent:
                jObject.Add("Link", JObject.FromObject(linkAddEvent.Link));
                break;
            case UserLinkRemoveEvent linkRemoveEvent:
                jObject.Add("Link", JObject.FromObject(linkRemoveEvent.Link));
                break;
            case UserProjectAddEvent projectAddEvent:
                jObject.Add("Project", JValue.CreateString(projectAddEvent.Project.ToString()));
                break;
            case UserProjectRemoveEvent projectRemoveEvent:
                jObject.Add("Project", JValue.CreateString(projectRemoveEvent.Project.ToString()));
                break;
            case UserPublisherAddEvent publisherAddEvent:
                jObject.Add("Publisher", JValue.CreateString(publisherAddEvent.Publisher.ToString()));
                break;
            case UserPublisherRemoveEvent publisherRemoveEvent:
                jObject.Add("Publisher", JValue.CreateString(publisherRemoveEvent.Publisher.ToString()));
                break;
        }
    }

    private static void WriteProject(WinAppCommunityUpdateEvent @event, JObject jObject)
    {
        switch (@event)
        {
            case ProjectNameUpdateEvent projectNameUpdateEvent:
                jObject.Add("Name", new JValue(projectNameUpdateEvent.Name));
                break;
            case ProjectDescriptionUpdateEvent projectDescriptionUpdateEvent:
                jObject.Add("Description", new JValue(projectDescriptionUpdateEvent.Description));
                break;
            case ProjectIconUpdateEvent projectIconUpdateEvent:
                jObject.Add("Icon",
                    projectIconUpdateEvent.Icon == null
                        ? null
                        : JValue.CreateString(projectIconUpdateEvent.Icon.ToString()));
                break;
            case ProjectHeroImageUpdateEvent projectHeroImageUpdateEvent:
                jObject.Add("HeroImage",
                    projectHeroImageUpdateEvent.HeroImage == null
                        ? null
                        : JValue.CreateString(projectHeroImageUpdateEvent.HeroImage.ToString()));
                break;
            case ProjectImageAddEvent projectImageAddEvent:
                jObject.Add("Image", JValue.CreateString(projectImageAddEvent.Image.ToString()));
                break;
            case ProjectImageRemoveEvent projectImageRemoveEvent:
                jObject.Add("Image", JValue.CreateString(projectImageRemoveEvent.Image.ToString()));
                break;
            case ProjectFeatureAddEvent projectFeatureAddEvent:
                jObject.Add("Feature", new JValue(projectFeatureAddEvent.Feature));
                break;
            case ProjectFeatureRemoveEvent projectFeatureRemoveEvent:
                jObject.Add("Feature", new JValue(projectFeatureRemoveEvent.Feature));
                break;
            case ProjectAccentColorUpdateEvent projectAccentColorUpdateEvent:
                jObject.Add("AccentColor",
                    projectAccentColorUpdateEvent.AccentColor == null
                        ? null
                        : new JValue(projectAccentColorUpdateEvent.AccentColor));
                break;
            case ProjectCategoryUpdateEvent projectCategoryUpdateEvent:
                jObject.Add("Category", new JValue(projectCategoryUpdateEvent.Category));
                break;
            case ProjectDependencyAddEvent projectDependencyAddEvent:
                jObject.Add("Dependency", JValue.CreateString(projectDependencyAddEvent.Dependency.ToString()));
                break;
            case ProjectDependencyRemoveEvent projectDependencyRemoveEvent:
                jObject.Add("Dependency", JValue.CreateString(projectDependencyRemoveEvent.Dependency.ToString()));
                break;
            case ProjectCollaboratorAddEvent projectCollaboratorAddEvent:
                jObject.Add("Collaborator", JObject.FromObject(projectCollaboratorAddEvent.Collaborator));
                break;
            case ProjectCollaboratorRemoveEvent projectCollaboratorRemoveEvent:
                jObject.Add("Collaborator", JObject.FromObject(projectCollaboratorRemoveEvent.Collaborator));
                break;
            case ProjectLinkAddEvent projectLinkAddEvent:
                jObject.Add("Link", JObject.FromObject(projectLinkAddEvent.Link));
                break;
            case ProjectLinkRemoveEvent projectLinkRemoveEvent:
                jObject.Add("Link", JObject.FromObject(projectLinkRemoveEvent.Link));
                break;
            case ProjectPublishedConnectionAddEvent projectConnectionAddEvent:
                jObject.Add("PublishedConnection", JObject.FromObject(projectConnectionAddEvent.Connection));
                break;
            case ProjectPublishedConnectionRemoveEvent projectConnectionRemoveEvent:
                jObject.Add("PublishedConnection", JObject.FromObject(projectConnectionRemoveEvent.Connection));
                break;
            case ProjectForgetMeUpdateEvent projectForgetMeUpdateEvent:
                jObject.Add("ForgetMe",
                    projectForgetMeUpdateEvent.ForgetMe == null
                        ? null
                        : new JValue(projectForgetMeUpdateEvent.ForgetMe.Value));
                break;
            case ProjectPrivacyUpdateEvent projectPrivacyUpdateEvent:
                jObject.Add("IsPrivate", new JValue(projectPrivacyUpdateEvent.IsPrivate));
                break;
        }
    }

    private static void WritePublisher(WinAppCommunityUpdateEvent @event, JObject jObject)
    {
        switch (@event)
        {
            case PublisherNameUpdateEvent publisherNameUpdateEvent:
                jObject.Add("Name", new JValue(publisherNameUpdateEvent.Name));
                break;
            case PublisherOwnerUpdateEvent ownerUpdateEvent:
                jObject.Add("Owner", new JValue(ownerUpdateEvent.Owner));
                break;
            case PublisherDescriptionUpdateEvent descriptionUpdateEvent:
                jObject.Add("Description", new JValue(descriptionUpdateEvent.Description));
                break;
            case PublisherIconUpdateEvent iconUpdateEvent:
                jObject.Add("Icon", iconUpdateEvent.Icon == null ? null : JValue.CreateString(iconUpdateEvent.Icon.ToString()));
                break;
            case PublisherAccentColorUpdateEvent accentColorUpdateEvent:
                jObject.Add("AccentColor", new JValue(accentColorUpdateEvent.AccentColor));
                break;
            case PublisherLinkAddEvent linkAddEvent:
                jObject.Add("Link", JObject.FromObject(linkAddEvent.Link));
                break;
            case PublisherLinkRemoveEvent linkRemoveEvent:
                jObject.Add("Link", JObject.FromObject(linkRemoveEvent.Link));
                break;
            case PublisherProjectAddEvent projectAddEvent:
                jObject.Add("Project", JValue.CreateString(projectAddEvent.Project.ToString()));
                break;
            case PublisherProjectRemoveEvent projectRemoveEvent:
                jObject.Add("Project", JValue.CreateString(projectRemoveEvent.Project.ToString()));
                break;
            case PublisherUserAddEvent userAddEvent:
                jObject.Add("User", JValue.CreateString(userAddEvent.User.ToString()));
                break;
            case PublisherUserRemoveEvent userRemoveEvent:
                jObject.Add("User", JValue.CreateString(userRemoveEvent.User.ToString()));
                break;
            case PublisherChildPublisherAddEvent childPublisherAddEvent:
                jObject.Add("ChildPublisher", JValue.CreateString(childPublisherAddEvent.ChildPublisher.ToString()));
                break;
            case PublisherChildPublisherRemoveEvent childPublisherRemoveEvent:
                jObject.Add("ChildPublisher", JValue.CreateString(childPublisherRemoveEvent.ChildPublisher.ToString()));
                break;
            case PublisherParentPublisherAddEvent parentPublisherAddEvent:
                jObject.Add("ParentPublisher", JValue.CreateString(parentPublisherAddEvent.ParentPublisher.ToString()));
                break;
            case PublisherParentPublisherRemoveEvent parentPublisherRemoveEvent:
                jObject.Add("ParentPublisher", JValue.CreateString(parentPublisherRemoveEvent.ParentPublisher.ToString()));
                break;
        }
    }
}
