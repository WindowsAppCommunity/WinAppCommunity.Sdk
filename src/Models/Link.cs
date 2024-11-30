namespace WinAppCommunity.Sdk.Models;

/// <summary>
/// Represents the data for a link.
/// </summary>
public record Link
{
    /// <summary>
    /// The external url this link points to.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// A display name for this url.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A description of this url (for accessibility or display).
    /// </summary>
    public required string Description { get; set; }
}
