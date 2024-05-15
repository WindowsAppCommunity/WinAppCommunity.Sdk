using Newtonsoft.Json;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents the role of a user.
/// </summary>
public record Role : IName
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
    public string Name { get; }

    /// <summary>
    /// A description for the role.
    /// </summary>
    public string Description { get; }

    /// <inheritdoc />
    public virtual bool Equals(Role? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Description == other.Description;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ Description.GetHashCode();
        }
    }
}
