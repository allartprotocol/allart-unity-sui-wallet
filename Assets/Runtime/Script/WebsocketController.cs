using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class WebsocketController : MonoBehaviour
{
    WebSocket websocket;
    public Action onWSEvent;

    public static WebsocketController instance;

    private void Awake()
    {
        instance = this;
    }

    public async void SetupConnection(string url)
    {
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            onWSEvent?.Invoke();
        };

        //InvokeRepeating("SendWebSocketMessage", 0.0f, 5f);

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void Subscribe(string parameter)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"jsonrpc\":\"2.0\", \"id\": 1, \"method\": \"sui_subscribeEvent\", \"params\": [" + parameter + "]}");
        }
    }

    public async void Unsubscribe(string parameter)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"jsonrpc\":\"2.0\", \"id\":1, \"method\":\"sui_unsubscribeEvent\", \"params\":[" + parameter + "]}");
        }
    }

    async void Stop()
    {
        await websocket.Close();
    }

    private void OnDestroy()
    {
        websocket?.Close();
    }

    private void OnApplicationQuit()
    {
        websocket?.Close();
    }

}