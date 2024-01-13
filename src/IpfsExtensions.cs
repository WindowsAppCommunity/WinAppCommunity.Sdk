using CommunityToolkit.Diagnostics;
using Ipfs;
using Ipfs.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WinAppCommunity.Sdk;

public static class IpfsExtensions
{
    /// <summary>
    /// Resolves the provided <paramref name="ipnsCid"/> as an Ipns address and retrieves the content from the DAG.
    /// </summary>
    /// <param name="ipnsCid">The cid of the DAG object to retrieve.</param>
    /// <param name="client">A client that can be used to communicate with Ipfs.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing task.</param>
    /// <returns>The deserialized DAG content, if any.</returns>
    public static async Task<TResult> ResolveIpnsDagAsync<TResult>(this Cid ipnsCid, IpfsClient client, CancellationToken cancellationToken)
    {
        var ipnsResResult = await client.Name.ResolveAsync($"/ipns/{ipnsCid.Hash}", recursive: true, cancel: cancellationToken);

        var resolvedCid = Cid.Decode(ipnsResResult.Replace("/ipfs/", ""));
        var projectRes = await client.Dag.GetAsync<TResult>(resolvedCid, cancellationToken);

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
}
