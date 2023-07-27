using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AllArt.SUI.RPC.Response;
using AllArt.SUI.RPC.Filter.Types;

public class WebsocketController: MonoBehaviour
{
    WebSocket websocket;
    public Action onWSEvent;

    public static WebsocketController instance;
    private ulong subId = 0;

    public void Awake()
    {
        instance = this;
    }

    public async void SetupConnection(string url)
    {
        if (url.Contains("https://"))
            url = url.Replace("https://", "wss://");

        websocket = new WebSocket(url);
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
            // Debug.Log("OnMessage! " + message);
            var response = JsonConvert.DeserializeObject<JsonRpcResponse<string>>(message);
            if(response.result != null)
            {
                subId = ulong.TryParse(response.result, out ulong id) ? id : 0;
                // Debug.Log(subId);
            }
            onWSEvent?.Invoke();
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

    }

    public async Task Subscribe(object filterParams)
    {
        if(websocket.State == WebSocketState.Closed)
        {
            await websocket.Connect();
        }

        if (websocket.State == WebSocketState.Open)
        {
            EventFilter filter = new("suix_subscribeTransaction", new List<object>{filterParams}); //{filterParams}

            string filterString = JsonConvert.SerializeObject(filter);
            // Debug.Log(filterString);
            await websocket.SendText(filterString);
        }
    }

    public void UnsubscribeCurrent()
    {
        if (websocket.State == WebSocketState.Open)
        {
            if(subId != 0)
                Unsubscribe(subId.ToString());
        }
    }

    public async void Unsubscribe(string id)
    {
        if (websocket.State == WebSocketState.Open)
        {
            EventFilter filter = new("suix_unsubscribeEvent", new List<object> { ulong.Parse(id) });
            string filterString = JsonConvert.SerializeObject(filter);
            Debug.Log(filterString);
            await websocket.SendText(filterString);
        }
    }

    public async void Stop()
    {
        await websocket.Close();
    }

    
}

