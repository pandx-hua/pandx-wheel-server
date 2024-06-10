using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace pandx.Wheel.SignalR;

public class InMemoryOnlineClientStore : IOnlineClientStore
{
    public InMemoryOnlineClientStore()
    {
        Clients = new ConcurrentDictionary<string, IOnlineClient>();
    }

    public ConcurrentDictionary<string, IOnlineClient> Clients { get; }

    public void Add(IOnlineClient client)
    {
        Clients.AddOrUpdate(client.ConnectionId, client, (x, y) => client);
    }

    public bool Remove(string connectionId)
    {
        return TryRemove(connectionId, out var removed);
    }

    public bool TryRemove(string connectionId, out IOnlineClient? client)
    {
        return Clients.TryRemove(connectionId, out client);
    }

    public bool TryGet(string connectionId, out IOnlineClient? client)
    {
        return Clients.TryGetValue(connectionId, out client);
    }

    public bool Contains(string connectionId)
    {
        return Clients.ContainsKey(connectionId);
    }

    public IReadOnlyList<IOnlineClient> GetAll()
    {
        return Clients.Values.ToImmutableList();
    }
}