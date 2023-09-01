using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FractalSDK.Models;
using UnityEngine.Networking;

namespace FractalSDK.Core
{
    public static class RestClient
    {
        public static async Task<Response> Get(string url, IEnumerable<RequestHeader> headers = null)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            if (headers != null)
            {
                foreach (RequestHeader header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            var getRequest = webRequest.SendWebRequest();

            while (!getRequest.isDone)
                await Task.Yield();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    return (new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Data = webRequest.downloadHandler.text,
                    });
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                default:
                    return (new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Error = webRequest.error
                    });
            }
        }

        public static async Task<Response> Post(string url, string body, IEnumerable<RequestHeader> headers = null)
        {
            byte[] bodyPayload = Encoding.UTF8.GetBytes(body);
            using UnityWebRequest webRequest = new(url, "POST");
            if (headers != null)
            {
                foreach (RequestHeader header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");

            webRequest.uploadHandler = new UploadHandlerRaw(bodyPayload);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            var postRequest = webRequest.SendWebRequest();

            while (!postRequest.isDone)
                await Task.Yield();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    return (new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Data = webRequest.downloadHandler.text,
                    });
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                default:
                    return (new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Error = webRequest.error,
                        Data = webRequest.downloadHandler.text,
                    });
            }
        }
    }
}
