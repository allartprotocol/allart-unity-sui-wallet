using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class WebsocketController: MonoBehaviour
{
    WebSocket websocket;
    public Action onWSEvent;

    public static WebsocketController instance;

    public void Awake()
    {
        instance = this;
    }

    public async void SetupConnection(string url)
    {
        if (url.Contains("https://"))
            url = url.Replace("https://", "wss://");

        websocket = new WebSocket("wss://fullnode.testnet.sui.io/");
        Debug.Log(url);

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
            Debug.Log("Connection closed! " + e);
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);
            onWSEvent?.Invoke();
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
        if(websocket != null)
            Debug.Log(websocket.State);

    }

    public async Task Subscribe(object filterParams)
    {
        if (websocket.State == WebSocketState.Open)
        {
            EventFilter filter = new("suix_subscribeTransaction", new List<object>{filterParams}); //{filterParams}

            string filterString = JsonConvert.SerializeObject(filter);
            Debug.Log(filterString);
            await websocket.SendText(filterString);
        }
    }

    public async void Unsubscribe(string id)
    {
        if (websocket.State == WebSocketState.Open)
        {
            EventFilter filter = new("suix_unsubscribeEvent", new List<object> { id });
            string filterString = JsonConvert.SerializeObject(filter);
            await websocket.SendText(filterString);
        }
    }

    public async void Stop()
    {
        await websocket.Close();
    }

    
}

