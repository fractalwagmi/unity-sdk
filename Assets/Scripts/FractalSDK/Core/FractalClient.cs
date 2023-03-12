using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FractalSDK.Enums;
using FractalSDK.Models;
using FractalSDK.Models.Api;
using UnityEngine;

namespace FractalSDK.Core
{
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
                        GameObject fractalClient = new()
                        {
                            name = nameof(FractalClient)
                        };
                        _instance = fractalClient.AddComponent<FractalClient>();
                        DontDestroyOnLoad(fractalClient);
                    }
                }
                return _instance;
            }
        }

        private string _clientId;

        private Scope[] _scopes;

        private string _bearerToken;
        
        public void Init(string clientId, Scope[] scopes)
        {
            _clientId = clientId;
            _scopes = scopes;
        }

        /// <summary>
        /// Returns a URL and code which are used to authenticate players.
        /// </summary>
        public async Task<AuthResponse> GetAuthUrl()
        {
            if (_clientId != null)
            {
                NameValueCollection requestQuery = new() { { "clientId", _clientId } };
                foreach (Scope scope in _scopes)
                {
                    requestQuery.Add("scope", FractalUtils.ToEnumString(scope));
                }

                string requestUrl = FractalConstants.AuthAPIRootURL + FractalConstants.GetURL + FractalUtils.ToQueryString(requestQuery);
                Response result = await RestClient.Get(requestUrl);

                if (result.StatusCode == 200)
                {
                    try
                    {
                        AuthResponse authUrlResponse = JsonUtility.FromJson<AuthResponse>(result.Data);
                        return authUrlResponse;
                    }
                    catch
                    {
                        throw new FractalInvalidResponse();
                    }
                }
                else
                {
                    throw new FractalAPIRequestError(result.StatusCode);
                }
            }
            else
            {
                throw new FractalInvalidClientId(_clientId);
            }
        }
        
        /// <summary>
        /// Returns the status of a player auth request.
        /// </summary>
        /// <param name="code">A authentication code to verify.</param>
        public async Task<ResultResponse> GetAuthResult(string code)
        {
            GetResult requestBody = new()
            {
                clientId = _clientId,
                code = code
            };

            const string requestUrl = FractalConstants.AuthAPIRootURL + FractalConstants.Verify;
            Response result = await RestClient.Post(requestUrl, JsonUtility.ToJson(requestBody));

            if (result.StatusCode == 200)
            {
                try
                {
                    ResultResponse resultResponse = JsonUtility.FromJson<ResultResponse>(result.Data);
                    _bearerToken = resultResponse.bearerToken;
                    return resultResponse;
                }
                catch
                {
                    throw new FractalInvalidResponse();
                }
            }
            else
            {
                throw new FractalAPIRequestError(result.StatusCode);
            }
        }
        
        /// <summary>
        /// Returns info about a given Fractal wallet. Requires user access token with the following scope: IDENTIFY
        /// </summary>
        public async Task<UserInfo> GetUser()
        {
            if (_scopes.Contains(Scope.IDENTIFY) && _bearerToken != null)
            {
                RequestHeader authorizationHeader = new()
                {
                    Key = "Authorization",
                    Value = "Bearer " + _bearerToken
                };

                const string requestUrl = FractalConstants.APIRootURL + FractalConstants.GetInfo;
                Response result = await RestClient.Get(requestUrl, new List<RequestHeader> { authorizationHeader });

                if (result.StatusCode == 200)
                {
                    try
                    {
                        UserInfo resultResponse = JsonUtility.FromJson<UserInfo>(result.Data);
                        return resultResponse;
                    }
                    catch
                    {
                        throw new FractalInvalidResponse();
                    }
                }
                else
                {
                    throw new FractalAPIRequestError(result.StatusCode);
                }
            }
            else
            {
                throw new FractalNotAuthenticated();
            }
        }
        
        /// <summary>
        /// Returns coins held in a given wallet. Requires user access token with the following scope: COINS_READ
        /// </summary>
        public async Task<UserCoins> GetCoins()
        {
            if (_scopes.Contains(Scope.COINS_READ) && _bearerToken != null)
            {
                RequestHeader authorizationHeader = new()
                {
                    Key = "Authorization",
                    Value = "Bearer " + _bearerToken
                };

                const string requestUrl = FractalConstants.APIRootURL + FractalConstants.GetCoins;
                Response result = await RestClient.Get(requestUrl, new List<RequestHeader> { authorizationHeader });

                if (result.StatusCode == 200)
                {
                    try
                    {
                        UserCoins resultResponse = JsonUtility.FromJson<UserCoins>(result.Data);
                        return resultResponse;
                    }
                    catch
                    {
                        throw new FractalInvalidResponse();
                    }
                }
                else
                {
                    throw new FractalAPIRequestError(result.StatusCode);
                }
            }
            else
            {
                throw new FractalNotAuthenticated();
            }
        }

        /// <summary>
        /// Returns items held in a given wallet. Requires user access token with the following scope: ITEMS_READ
        /// </summary>
        public async Task<UserItems> GetItems()
        {
            if (_scopes.Contains(Scope.ITEMS_READ) && _bearerToken != null)
            {
                RequestHeader authorizationHeader = new()
                {
                    Key = "Authorization",
                    Value = "Bearer " + _bearerToken
                };

                const string requestUrl = FractalConstants.APIRootURL + FractalConstants.GetItems;
                var result = await RestClient.Get(requestUrl, new List<RequestHeader> { authorizationHeader });

                if (result.StatusCode == 200)
                {
                    try
                    {
                        UserItems resultResponse = JsonUtility.FromJson<UserItems>(result.Data);
                        return resultResponse;
                    }
                    catch
                    {
                        throw new FractalInvalidResponse();
                    }
                }
                else
                {
                    throw new FractalAPIRequestError(result.StatusCode);
                }
            }
            else
            {
                throw new FractalNotAuthenticated();
            }
        }
    }
}
