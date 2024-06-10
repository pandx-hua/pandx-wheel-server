using pandx.Wheel.Authorization;
using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.SignalR;

public interface IOnlineClientManager : ISingletonDependency
{
    event EventHandler<OnlineClientEventArgs> ClientConnected;
    event EventHandler<OnlineClientEventArgs> ClientDisconnected;
    event EventHandler<OnlineUserEventArgs> UserConnected;
    event EventHandler<OnlineUserEventArgs> UserDisconnected;
    void Add(IOnlineClient client);
    bool Remove(string connectionId);
    IOnlineClient? GetByConnectionId(string connectionId);
    IReadOnlyList<IOnlineClient> GetAll();
    IReadOnlyList<IOnlineClient> GetAllByUser(IUserIdentifier userIdentifier);
}