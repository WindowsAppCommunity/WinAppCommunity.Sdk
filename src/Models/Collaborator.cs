using Ipfs;
using System.Text.Json.Serialization;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a collaborator on a <see cref="Models.Project"/>.
/// </summary>
public record Collaborator
{
    /// <summary>
    /// Creates a new instance of <see cref="Collaborator"/>.
    /// </summary>
    [JsonConstructor]
    public Collaborator(Cid user, Role role)
    {
        User = user;
        Role = role;
    }

    /// <summary>
    /// A <see cref="Cid"/> pointing to the <see cref="Models.User"/> that this refers to.
    /// </summary>
    public Cid User { get; set; }

    /// <summary>
    /// The role for the <see cref="User"/>.
    /// </summary>
    Role Role { get; set; }
}
