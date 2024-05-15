using Ipfs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents a collaborator on a <see cref="Models.Project"/>.
/// </summary>
public record Collaborator : IEqualityComparer<Collaborator>
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
    public Role Role { get; set; }

    /// <inheritdoc />
    public virtual bool Equals(Collaborator? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return User.Equals(other.User) && Role.Equals(other.Role);
    }

    /// <inheritdoc />
    public bool Equals(Collaborator x, Collaborator y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.User.Equals(y.User) && x.Role.Equals(y.Role);
    }

    /// <inheritdoc />
    public override int GetHashCode() => GetHashCode(this);

    /// <inheritdoc />
    public int GetHashCode(Collaborator obj)
    {
        unchecked
        {
            return (obj.User.GetHashCode() * 397) ^ obj.Role.GetHashCode();
        }
    }
}
