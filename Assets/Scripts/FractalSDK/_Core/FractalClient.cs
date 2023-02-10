using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FractalClient : MonoBehaviour
{
    private static FractalClient _instance;
    public static FractalClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FractalClient>();
                if (_instance == null)
                {
                    GameObject go = new();
                    go.name = typeof(FractalClient).Name;
                    _instance = go.AddComponent<FractalClient>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private string clientId;

    private Scope[] scopes;

    private string bearerToken;


    public void Init(string _clientId, Scope[] _scopes)
    {
        clientId = _clientId;
        scopes = _scopes;
    }

    /// <summary>
    /// Returns a URL and code which are used to authenticate players.
    /// </summary>
    /// <param name="OnSuccess">Callback when the request has sucessfully completed.</param>
    public void GetAuthUrl(System.Action<AuthResponse> OnSuccess)
    {
        NameValueCollection RequestQuerry = new();
        RequestQuerry.Add("clientId", clientId);
        foreach (Scope scope in scopes)
        {
            RequestQuerry.Add("scope", FractalUtils.ToEnumString(scope));
        }

        string requestUrl = FractalConstants.AUTH_API_ROOT_URL + FractalConstants.GET_URL + FractalUtils.ToQueryString(RequestQuerry);
        StartCoroutine(Get(requestUrl, callback =>
        {
            if (callback.StatusCode == 200)
            {
                AuthResponse authUrlResponse = JsonUtility.FromJson<AuthResponse>(callback.Data);
                OnSuccess(authUrlResponse);
            }
        }));
    }


    /// <summary>
    /// Returns the status of a player auth request.
    /// </summary>
    /// <param name="code">A authentication code to verify.</param>
    /// <param name="OnSuccess">Callback when the request has sucessfully completed.</param>
    public void GetAuthResult(string code, System.Action<ResultResponse> OnSuccess)
    {
        GetResult RequestBody = new();
        RequestBody.clientId = clientId;
        RequestBody.code = code;

        string requestUrl = FractalConstants.AUTH_API_ROOT_URL + FractalConstants.VERIFY;
        StartCoroutine(Post(requestUrl, JsonUtility.ToJson(RequestBody), callback =>
        {
            if (callback.StatusCode == 200)
            {
                ResultResponse resultResponse = JsonUtility.FromJson<ResultResponse>(callback.Data);
                bearerToken = resultResponse.bearerToken;
                OnSuccess(resultResponse);
            }
        }));
    }


    /// <summary>
    /// Returns info about a given Fractal wallet. Requires user access token with the following scope: IDENTIFY
    /// </summary>
    /// <param name="OnSuccess">Callback when the request has sucessfully completed.</param>
    public void GetUser(System.Action<UserInfo> OnSuccess)
    {
        if (scopes.Contains(Scope.IDENTIFY) && bearerToken != null)
        {
            RequestHeader authorizationHeader = new()
        {
            Key = "Authorization",
            Value = "Bearer " + bearerToken
        };

        string requestUrl = FractalConstants.SDK_API_ROOT_URL + FractalConstants.GET_INFO;
        StartCoroutine(Get(requestUrl, callback =>
        {
            if (callback.StatusCode == 200)
            {
                UserInfo resultResponse = JsonUtility.FromJson<UserInfo>(callback.Data);
                OnSuccess(resultResponse);
            }
        }, new List<RequestHeader>{authorizationHeader}));
        }
        else
        {
            FractalUtils.Log("Not Authenticated / Missing Scope [IDENTIFY]");
        }
    }


    /// <summary>
    /// Returns coins held in a given wallet. Requires user access token with the following scope: COINS_READ
    /// </summary>
    /// <param name="OnSuccess">Callback when the request has sucessfully completed.</param>
    public void GetCoins(System.Action<UserCoins> OnSuccess)
    {
        if (scopes.Contains(Scope.COINS_READ) && bearerToken != null)
        {
            RequestHeader authorizationHeader = new()
            {
                Key = "Authorization",
                Value = "Bearer " + bearerToken
            };

            string requestUrl = FractalConstants.SDK_API_ROOT_URL + FractalConstants.GET_COINS;
            StartCoroutine(Get(requestUrl, callback =>
            {
                if (callback.StatusCode == 200)
                {
                    UserCoins resultResponse = JsonUtility.FromJson<UserCoins>(callback.Data);
                    OnSuccess(resultResponse);
                }
            }, new List<RequestHeader> { authorizationHeader }));
        }
        else
        {
            FractalUtils.Log("Not Authenticated / Missing Scope [COINS_READ]");
        }
    }

    /// <summary>
    /// Returns items held in a given wallet. Requires user access token with the following scope: ITEMS_READ
    /// </summary>
    /// <param name="OnSuccess">Callback when the request has sucessfully completed.</param>
    public void GetItems(System.Action<UserItems> OnSuccess)
    {
        if (scopes.Contains(Scope.ITEMS_READ) && bearerToken != null)
        {
            RequestHeader authorizationHeader = new()
            {
                Key = "Authorization",
                Value = "Bearer " + bearerToken
            };

            string requestUrl = FractalConstants.SDK_API_ROOT_URL + FractalConstants.GET_ITEMS;
            StartCoroutine(Get(requestUrl, callback =>
            {
                if (callback.StatusCode == 200)
                {
                    UserItems resultResponse = JsonUtility.FromJson<UserItems>(callback.Data);
                    OnSuccess(resultResponse);
                }
            }, new List<RequestHeader> { authorizationHeader }));
        }
        else
        {
            FractalUtils.Log("Not Authenticated / Missing Scope [ITEMS_READ]");
        }
    }


    private IEnumerator Get(string url, System.Action<Response> Callback, IEnumerable<RequestHeader> headers = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            if (headers != null)
            {
                foreach (RequestHeader header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Callback(new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Error = webRequest.error
                    });
                    break;
                case UnityWebRequest.Result.Success:
                    Callback(new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Data = webRequest.downloadHandler.text,
                    });
                    break;
            }
        }
    }

    private IEnumerator Post(string url, string body, System.Action<Response> Callback, IEnumerable<RequestHeader> headers = null)
    {
        byte[] bodyPayload = Encoding.UTF8.GetBytes(body);
        using (UnityWebRequest webRequest = new(url, "POST"))
        {
            if (headers != null)
            {
                foreach (RequestHeader header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.uploadHandler = new UploadHandlerRaw(bodyPayload);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Callback(new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Error = webRequest.error
                    });
                    break;
                case UnityWebRequest.Result.Success:
                    Callback(new Response
                    {
                        StatusCode = webRequest.responseCode,
                        Data = webRequest.downloadHandler.text,
                    });
                    break;
            }
        }
    }

}
