﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FractalSDK.Enums;
using FractalSDK.Models;
using FractalSDK.Models.Api;
using UnityEditor;
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

        private FractalConfig _config;
        private Scope[] _scopes;

        private string _bearerToken;
        
        public void Init(Scope[] scopes)
        {
            const string configName = "FractalConfig";
            _config = Instantiate(Resources.Load(configName) as FractalConfig);

            if (_config == null)
            {
                FractalUtils.LogError("Configuration has not been found, create FractalConfig from Context menu in the Assets/Resources folder to be loaded at runtime");
            }
            
            _scopes = scopes;
        }

        /// <summary>
        /// Returns a URL and code which are used to authenticate players.
        /// </summary>
        public async Task<AuthResponse> GetAuthUrl(string codeChallenge)
        {
            if (_config.clientId != null)
            {
                List<string> scopesString = new List<string>();
                foreach (Scope scope in _scopes)
                {
                    scopesString.Add(FractalUtils.ToEnumString(scope));
                }


                RequestUrl requestBody = new()
                {
                    clientId = _config.clientId,
                    codeChallenge = codeChallenge,
                    scopes = scopesString
            };

                const string requestUrl = FractalConstants.AuthAPIRootURL + FractalConstants.GetURL;
                FractalUtils.Log(JsonUtility.ToJson(requestBody));
                Response result = await RestClient.Post(requestUrl, JsonUtility.ToJson(requestBody));

                if (result.StatusCode == 200)
                {
                    try
                    {
                        AuthResponse resultResponse = JsonUtility.FromJson<AuthResponse>(result.Data);
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
                throw new FractalInvalidClientId(_config.clientId);
            }
        }
        
        /// <summary>
        /// Returns the status of a player auth request.
        /// </summary>
        /// <param name="code">A authentication code to verify.</param>
        public async Task<ResultResponse> GetAuthResult(string verifier)
        {
            GetResult requestBody = new()
            {
                verifier = verifier
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


        /// <summary>
        /// Generates a url and code where players can approve a transaction signing request.
        /// </summary>
        /// <param name="_unsignedMessage">Base58 encoded unsigned message</param>
        /// <returns>URL and Code to sign and verify the transaction.</returns>
        public async Task<TransactionUrl> SignTransaction(string _unsignedMessage)
        {
            if (_bearerToken != null)
            {
                RequestHeader authorizationHeader = new()
                {
                    Key = "Authorization",
                    Value = "Bearer " + _bearerToken
                };

                UnsignedTransaction requestBody = new()
                {
                    unsignedMessage = _unsignedMessage
                };

                const string requestUrl = FractalConstants.APIRootURL + FractalConstants.SignTransaction;
                FractalUtils.Log(requestUrl);
                var result = await RestClient.Post(requestUrl, JsonUtility.ToJson(requestBody), new List<RequestHeader> { authorizationHeader });

                if (result.StatusCode == 200)
                {
                    try
                    {
                        TransactionUrl resultResponse = JsonUtility.FromJson<TransactionUrl>(result.Data);
                        return resultResponse;
                    }
                    catch
                    {
                        throw new FractalInvalidResponse();
                    }
                }
                else
                {
                    UnsignedTransactionError error = JsonUtility.FromJson<UnsignedTransactionError>(result.Data);
                    throw new FractalAPIRequestError(error.code, error.message);
                }
            }
            else
            {
                throw new FractalNotAuthenticated();
            }
        }
    }
}
