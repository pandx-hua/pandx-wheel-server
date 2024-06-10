using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.SignalR;

public interface IOnlineClientStore : ISingletonDependency
{
    void Add(IOnlineClient client);
    bool Remove(string connectionId);
    bool TryRemove(string connectionId, out IOnlineClient? client);
    bool TryGet(string connectionId, out IOnlineClient? client);
    bool Contains(string connectionId);
    IReadOnlyList<IOnlineClient> GetAll();
}