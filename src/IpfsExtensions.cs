using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.CoreApi;

namespace WinAppCommunity.Sdk;

public static class IpfsExtensions
{
    /// <summary>
    /// Resolves the provided <paramref name="cid"/> as an Ipns address and retrieves the content from the DAG.
    /// </summary>
    /// <param name="cid">The cid of the DAG object to retrieve.</param>
    /// <param name="client">A client that can be used to communicate with Ipfs.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing task.</param>
    /// <returns>The deserialized DAG content, if any.</returns>
    public static async Task<TResult> ResolveIpnsDagAsync<TResult>(this Cid cid, IpfsClient client, CancellationToken cancellationToken)
    {
        if (cid.ContentType == "libp2p-key")
        {
            var ipnsResResult = await client.Name.ResolveAsync($"/ipns/{cid}", recursive: true, cancel: cancellationToken);

            cid = Cid.Decode(ipnsResResult.Replace("/ipfs/", ""));
        }

        var projectRes = await client.Dag.GetAsync<TResult>(cid, cancellationToken);

        Guard.IsNotNull(projectRes);
        return projectRes;
    }

    /// <summary>
    /// Resolves the provided <paramref name="cids"/> as Ipns addresses and retrieves the content from the DAG.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize to.</typeparam>
    /// <param name="cids">The IPNS CIDs of the Dag objects to retrieve.</param>
    /// <param name="client">A client that can be used to communicate with Ipfs.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing task.</param>
    /// <returns>An async enumerable that yields the requested data.</returns>
    public static IAsyncEnumerable<TResult> ResolveIpnsDagAsync<TResult>(this IEnumerable<Cid> cids, IpfsClient client, CancellationToken cancellationToken)
        => cids
            .ToAsyncEnumerable()
            .SelectAwaitWithCancellation(async (cid, cancel) => await cid.ResolveIpnsDagAsync<TResult>(client, CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancel).Token));

    /// <summary>
    /// Creates an ipns key using a temporary name, then renames it to match the Id of the key.
    /// </summary>
    /// <remarks>
    /// Enables pushing to ipns without additional calls to convert ipns cid to name.
    /// </remarks>
    /// <param name="keyApi">The key api to use for accessing ipfs keys.</param>
    /// <param name="size">The size of the key to create.</param>
    /// <returns>A task containing the created key.</returns>
    public static async Task<IKey> CreateKeyWithNameOfIdAsync(this IKeyApi keyApi, int size = 4096)
    {
        var key = await keyApi.CreateAsync(name: "temp", "ed25519", size);

        // Rename key name to the key id
        return await keyApi.RenameAsync("temp", $"{key.Id}");
    }
}
