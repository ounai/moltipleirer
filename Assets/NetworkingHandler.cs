using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class OtherPlayer {
    public int id;
    public float x;
    public float y;

    public OtherPlayer(int id, float x, float y) {
        this.id = id;
        this.x = x;
        this.y = y;
    }
}

public class NetworkingHandler : MonoBehaviour {
    public string ip = "51.195.222.59";
    public int port = 31337;
    private TextUDPClient textUDPClient;
    private NetworkingGUI networkingGUI;
    public List<OtherPlayer> otherPlayers = new List<OtherPlayer>();

    void HandleJoin(string packet) {
        networkingGUI.connectedPlayers++;

        int id = int.Parse(packet.Split('|')[1]);
        float x = float.Parse(packet.Split('|')[2]);
        float y = float.Parse(packet.Split('|')[3]);

        otherPlayers.Add(new OtherPlayer(id, x, y));
    }

    void HandleLeave(string packet) {
        networkingGUI.connectedPlayers--;
        // TODO
    }

    void OnReceive(string packet) {
        Debug.Log("Received: " + packet);
        networkingGUI.rx = packet;

        if (packet == "hello") networkingGUI.connectionStatus = "Connected";
        else if (packet == "goodbye") networkingGUI.connectionStatus = "Disconnected";
        else if (packet.StartsWith("joined")) HandleJoin(packet);
        else if (packet.StartsWith("left")) HandleLeave(packet);
    }

    void Send(string packet) {
        networkingGUI.tx = packet;

        textUDPClient.SendString(packet);
    }

    void Start() {
        networkingGUI = GetComponent<NetworkingGUI>();
        networkingGUI.connectionStatus = "Connecting";

        textUDPClient = new TextUDPClient(ip, port);
        textUDPClient.Connect(OnReceive);

        Send("join");
    }
}
