using System;

public static class GameplayEventBus
{
    // msg, senderClientId
    public static Action<NetworkJsonMessage, ulong> OnMessage;

    public static void Handle(NetworkJsonMessage msg, ulong sender)
    {
        OnMessage?.Invoke(msg, sender);
    }
}