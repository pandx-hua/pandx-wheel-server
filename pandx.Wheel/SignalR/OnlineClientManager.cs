using System.Collections.Immutable;
using pandx.Wheel.Authorization;

namespace pandx.Wheel.SignalR;

public class OnlineClientManager : IOnlineClientManager
{
    public readonly object SysObj = new();

    public OnlineClientManager(IOnlineClientStore store)
    {
        Store = store;
    }

    public IOnlineClientStore Store { get; }
    public event EventHandler<OnlineClientEventArgs>? ClientConnected;
    public event EventHandler<OnlineClientEventArgs>? ClientDisconnected;
    public event EventHandler<OnlineUserEventArgs>? UserConnected;
    public event EventHandler<OnlineUserEventArgs>? UserDisconnected;

    public void Add(IOnlineClient client)
    {
        lock (SysObj)
        {
            var userWasAlreadyOnline = false;
            var user = client.ToUserIdentifier();
            if (user is not null)
            {
                userWasAlreadyOnline = this.IsOnline(user);
            }

            Store.Add(client);
            ClientConnected?.Invoke(this, new OnlineClientEventArgs(client));
            if (user is not null && !userWasAlreadyOnline)
            {
                UserConnected?.Invoke(this, new OnlineUserEventArgs(user, client));
            }
        }
    }

    public bool Remove(string connectionId)
    {
        lock (SysObj)
        {
            var result = Store.TryRemove(connectionId, out var client);
            if (result)
            {
                if (UserDisconnected is { } userDisconnected)
                {
                    var user = client?.ToUserIdentifier();
                    if (user is not null && !this.IsOnline(user))
                    {
                        userDisconnected.Invoke(this,
                            new OnlineUserEventArgs(user, client ?? throw new InvalidOperationException()));
                    }
                }

                ClientDisconnected?.Invoke(this,
                    new OnlineClientEventArgs(client ?? throw new InvalidOperationException()));
            }

            return result;
        }
    }

    public IOnlineClient? GetByConnectionId(string connectionId)
    {
        lock (SysObj)
        {
            return Store.TryGet(connectionId, out var client) ? client : null;
        }
    }

    public IReadOnlyList<IOnlineClient> GetAll()
    {
        lock (SysObj)
        {
            return Store.GetAll();
        }
    }

    public IReadOnlyList<IOnlineClient> GetAllByUser(IUserIdentifier userIdentifier)
    {
        return GetAll().Where(x => x.UserId == userIdentifier.UserId).ToImmutableList();
    }
}