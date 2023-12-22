using CommunityToolkit.Diagnostics;
using Ipfs.Http;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using WinAppCommunity.Sdk.Models;

namespace WinAppCommunity.Sdk
{
    public static class ModelExtensions
    {
        public static async IAsyncEnumerable<Project> GetPublisherProjectsAsync(this Publisher publisher, IpfsClient client, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var cid in publisher.Projects.ToArray())
            {
                Guard.IsNotNullOrWhiteSpace(cid);

                var res = await client.Dag.GetAsync<Project>(cid, cancellationToken);
                if (res is not null)
                    yield return res;
            }
        }
    }
}
