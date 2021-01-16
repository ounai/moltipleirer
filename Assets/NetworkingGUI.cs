using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CustomTextGUI {
    private const float marginLeft = 10.0f, marginTop = 10.0f, height = 24.0f;
    private int screenWidth, screenHeight;
    private Rect rect;

    public CustomTextGUI(int screenWidth, int screenHeight) {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;

        this.Reset();
    }

    public void Reset() {
        rect = new Rect(marginLeft, marginTop, screenWidth, height);
    }

    public void Text(string text) {
        GUI.Label(rect, text);

        rect.yMin += height;
        rect.yMax += height;
    }
}

public class NetworkingGUI : MonoBehaviour {
    public string connectionStatus = "Not connected";
    public string rx, tx;
    private CustomTextGUI customTextGUI;
    public int connectedPlayers = 0;
    public Texture otherPlayerTexture;

    void Start() {
        customTextGUI = new CustomTextGUI(Screen.width, Screen.height);
    }

    void OnGUI() {
        customTextGUI.Reset();

        customTextGUI.Text("Connection status: " + connectionStatus);

        if (connectionStatus == "Connected") {
            customTextGUI.Text(connectedPlayers + " players connected");

            if (tx != null) customTextGUI.Text("tx: " + tx);
            if (rx != null) customTextGUI.Text("rx: " + rx);
        }

        List<OtherPlayer> otherPlayers = GetComponent<NetworkingHandler>().otherPlayers;

        foreach (OtherPlayer otherPlayer in otherPlayers) {
            GUI.Box(new Rect(otherPlayer.x, otherPlayer.y, otherPlayerTexture.width, otherPlayerTexture.height), otherPlayerTexture);
        }
    }
}
