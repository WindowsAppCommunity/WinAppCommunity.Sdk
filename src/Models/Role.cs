using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents the role of a user.
/// </summary>
public record Role
{
    /// <summary>
    /// Creates a new instance of <see cref="Role"/>.
    /// </summary>
    [JsonConstructor]
    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// The name of the role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A description for the role.
    /// </summary>
    public string Description { get; set; }
}
