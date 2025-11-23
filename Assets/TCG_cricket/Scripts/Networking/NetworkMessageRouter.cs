using System.Text;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkMessageRouter : MonoBehaviour
{
    public static NetworkMessageRouter Instance { get; private set; }
    private NetworkManager net => NetworkManager.Singleton;

    private const string ClientToServer = "ClientToServerMessage";
    private const string ServerToClient = "ServerToClientMessage";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Register handlers only once when NetworkManager exists
        if (net == null) return;

        if (net.IsServer)
        {
            net.CustomMessagingManager.RegisterNamedMessageHandler(ClientToServer, OnClientMessageReceived);
        }

        net.CustomMessagingManager.RegisterNamedMessageHandler(ServerToClient, OnServerMessageReceived);
    }

    private void OnDisable()
    {
        if (net == null) return;

        // Unregister handlers to avoid duplicates
        net.CustomMessagingManager.UnregisterNamedMessageHandler(ClientToServer);
        net.CustomMessagingManager.UnregisterNamedMessageHandler(ServerToClient);
    }

    // ----------------- Sending helpers -----------------

    // Client -> Server
    public void SendToServer(NetworkJsonMessage msg)
    {
        if (net == null) return;
        var json = JsonUtility.ToJson(msg);
        var bytes = Encoding.UTF8.GetBytes(json);
        using (var writer = new FastBufferWriter(bytes.Length, Allocator.Temp))
        {
            writer.WriteBytesSafe(bytes, bytes.Length);
            // Always send to server client id
            net.CustomMessagingManager
                .SendNamedMessage(ClientToServer,NetworkManager.ServerClientId, writer);
        }
    }

    // Server -> single client
    public void SendToClient(ulong clientId, NetworkJsonMessage msg)
    {
        if (net == null || !net.IsServer) return;
        var json = JsonUtility.ToJson(msg);
        var bytes = Encoding.UTF8.GetBytes(json);
        using (var writer = new FastBufferWriter(bytes.Length, Allocator.Temp))
        {
            writer.WriteBytesSafe(bytes, bytes.Length);
            net.CustomMessagingManager.SendNamedMessage(ServerToClient, clientId, writer);
        }
    }

    // Server -> broadcast to all clients
    public void BroadcastToClients(NetworkJsonMessage msg)
    {
        if (net == null || !net.IsServer) return;
        var json = JsonUtility.ToJson(msg);
        var bytes = Encoding.UTF8.GetBytes(json);
        using (var writer = new FastBufferWriter(bytes.Length, Allocator.Temp))
        {
            writer.WriteBytesSafe(bytes, bytes.Length);
            net.CustomMessagingManager.SendNamedMessage(ServerToClient, net.ConnectedClientsIds, writer);
        }
    }

    // ----------------- Receive handlers -----------------

    // Registered on the server: receives client → server messages
    private void OnClientMessageReceived(ulong senderClientId, FastBufferReader reader)
    {
        // Read all bytes
        byte[] bytes = null;
        reader.ReadBytesSafe(ref bytes, reader.Length);
        var json = Encoding.UTF8.GetString(bytes);
        NetworkJsonMessage msg = null;
        try { msg = JsonUtility.FromJson<NetworkJsonMessage>(json); }
        catch { Debug.LogWarning("Failed to parse JSON from client: " + json); }
        if (msg != null) GameplayEventBus.Handle(msg, senderClientId);
    }

    // Registered on everyone (server also has this handler for server->client named message)
    private void OnServerMessageReceived(ulong senderClientId, FastBufferReader reader)
    {
        byte[] bytes = null;
        reader.ReadBytesSafe(ref bytes, reader.Length);
        var json = Encoding.UTF8.GetString(bytes);
        NetworkJsonMessage msg = null;
        try { msg = JsonUtility.FromJson<NetworkJsonMessage>(json); }
        catch { Debug.LogWarning("Failed to parse JSON from server: " + json); }
        if (msg != null) GameplayEventBus.Handle(msg, senderClientId);
    }
}
