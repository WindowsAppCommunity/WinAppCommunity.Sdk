using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents the data for a link.
/// </summary>
public record Link
{
    /// <summary>
    /// Creates a new instance of <see cref="Link"/>.
    /// </summary>
    [JsonConstructor]
    public Link(string url, string name, string description)
    {
        Url = url;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// The external url this link points to.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// A display name for this url.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A description of this url (for accessibility or display).
    /// </summary>
    public string Description { get; set; }
}
