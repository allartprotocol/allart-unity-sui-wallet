using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using Solnet.Rpc.Messages;

public class RPCClient
{
    internal string _uri;

    public RPCClient(string uri)
    {
        _uri = uri;
    }

    internal async Task<JsonRpcResponse<T>> SendRequest<T>(object data)
    {
        var requestJson = JsonConvert.SerializeObject(data, new Newtonsoft.Json.Converters.StringEnumConverter());
        // Debug.Log(requestJson);
        var requestData = System.Text.Encoding.UTF8.GetBytes(requestJson);
        using var uwr = new UnityWebRequest(_uri, "POST");
        uwr.uploadHandler = new UploadHandlerRaw(requestData);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SendWebRequest();

        while (!uwr.isDone)
        {
            await Task.Yield();
        }

        // Debug.Log(uwr.downloadHandler.text);
        var response = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(uwr.downloadHandler.text);
        return response;
    }

    internal async Task<T> SendAPIRequest<T>(object data)
    {
        var requestJson = JsonConvert.SerializeObject(data, new Newtonsoft.Json.Converters.StringEnumConverter());
        Debug.Log(requestJson);
        var requestData = System.Text.Encoding.UTF8.GetBytes(requestJson);
        using (var uwr = new UnityWebRequest(_uri, "POST"))
        {
            uwr.uploadHandler = new UploadHandlerRaw(requestData);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                await Task.Yield();
            }

            Debug.Log(uwr.downloadHandler.text);
            var response = JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
            return response;
        }
    }

    public async Task<T> Get<T>(string url)
    {
        using var uwr = new UnityWebRequest(url, "GET");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SendWebRequest();

        while (!uwr.isDone)
        {
            await Task.Yield();
        }

        var response = JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
        return response;
    }

    public async Task<Sprite> DownloadImage(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                await Task.Yield();
            }

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);                
                return sprite;
            }
            else
            {
                Debug.Log(uwr.error);
            }
        }

        return null;
    }

    public IEnumerator SendRequestCoroutine(string uri, Action<UnityWebRequest> callback)
    {
        using (var request = UnityWebRequest.Get(uri))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            var asyncOp = request.SendWebRequest();

            while (!asyncOp.isDone)
            {
                yield return null;
            }

            callback(request);
        }
    }

}
