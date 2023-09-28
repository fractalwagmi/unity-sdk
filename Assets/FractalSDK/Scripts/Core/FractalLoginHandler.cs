using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FractalSDK.Enums;
using FractalSDK.Models.Api;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FractalSDK.Core
{
    public class FractalLoginHandler : MonoBehaviour
    {
        #region Fields

        [Tooltip("Button object to manually initiate the login process.")]
        public Button loginButton;

        [Tooltip("Text mesh to show user's account e-mail.")]
        public TextMeshProUGUI authUserText;

        [Tooltip("Specify the scopes that the game will request from the API.")]
        public Scope[] scopes;

        [Tooltip("Event to call when user has begin authentication.")]
        public UnityEvent onStarted;

        [Tooltip("Event to call when user is successfully signed in.")]
        public UnityEvent onVerified;

        [Tooltip("Event to call when authentication failed and expired.")]
        public UnityEvent onError;

        #endregion

        #region Externals

#if !UNITY_SWITCH && !UNITY_PS4 && !UNITY_PS4 && !UNITY_XBOXONE
        //[External JS Call]
        [DllImport("__Internal")]
        private static extern void SetupFractalEvents();

        //[External JS Call]
        [DllImport("__Internal")]
        private static extern void OpenFractalPopup(string url);

        //[External JS Call]
        [DllImport("__Internal")]
        private static extern void CloseFractalPopup();

#else
        private static void SetupFractalEvents()
        {
            throw new NotImplementedException();
        }

        private static void OpenFractalPopup(string url)
        {
            throw new NotImplementedException();
        }

        private static void CloseFractalPopup()
        {
            throw new NotImplementedException();
        }

#endif

        #endregion


        private string _loginCode;


        void Start()
        {
            Button loginPrefab = loginButton.GetComponent<Button>();
            loginPrefab.onClick.AddListener(InitAuth);
            FractalClient.Instance.Init(scopes);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SetupFractalEvents();
            }            
        }


        /// <summary>
        /// Initializes Fractal authentication process. 
        /// </summary>
        private async void InitAuth()
        {
            FractalCodeChallenge pair = new FractalCodeChallenge();
            pair.Init();

            onStarted?.Invoke();
            try
            {
                AuthResponse authUrl = await FractalClient.Instance.GetAuthUrl(pair.CodeChallenge);
                OpenAuth(authUrl, pair);
            }
            catch (Exception ex)
            {
                onError?.Invoke();
                FractalUtils.Log(ex.ToString());
            }

        }

        /// <summary>
        /// Opens Fractal authentication URL in the systems default browser.
        /// On WebGL a Fractal plugin is used to open the authentication in popup.
        /// </summary>
        /// <param name="codeChallange">Authentication URL to open.</param>
        private void OpenAuth(AuthResponse authUrl, FractalCodeChallenge codeChallenge)
        {
            authUserText.text = FractalConstants.ButtonLoading;
            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                    OpenFractalPopup(authUrl.approvalUrl);
                    break;

                default:
                    Process[] pname = Process.GetProcessesByName("fractal");
                    if (pname.Length != 0) { 
                        Application.OpenURL("fractal://signinv2/approve?challenge=" + codeChallenge.CodeChallenge);
                        LoginPooler(codeChallenge.CodeVerifier);
                    }
                    else {
                        Application.OpenURL(authUrl.approvalUrl);
                        LoginPooler(codeChallenge.CodeVerifier);
                    }
                    break;
                    
            }
        }

        /// <summary>
        /// Handles the callback messages from the Fractal auth popup (WebGL only)
        /// </summary>
        /// <param name="payload">Message received from the WebGL popup.</param>
        public async void HandlePopupMessage(string payload)
        {
            FractalUtils.Log("Message from popup recieved" + payload);
            switch (payload){
                case "PROJECT_APPROVED":
                    try
                    {
                        CloseFractalPopup();
                        ResultResponse result = await FractalClient.Instance.GetAuthResult(_loginCode);
                        OnFinishedVerification(result);
                    }
                    catch
                    {
                        OnFailedVerification();
                    }
                    break;
                case "POPUP_CLOSED":
                    OnFailedVerification();
                    break;
            }
        }

        /// <summary>
        /// Pools if the user finished the authentication in the popup.
        /// </summary>
        /// <param name="code">Authentication code to validate.</param>
        private async void LoginPooler(string code)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Application.isFocused)
                {
                    try
                    {
                        ResultResponse result = await FractalClient.Instance.GetAuthResult(Convert.ToBase64String(Encoding.UTF8.GetBytes(code)));
                        OnFinishedVerification(result);
                        return;
                    }
                    catch
                    {
                        await Task.Delay(2000);
                        authUserText.text = FractalConstants.ButtonLoading;
                    }
                }
            }
            OnFailedVerification();
        }

        private void OnFinishedVerification(ResultResponse resultResponse)
        {
            FractalUtils.Log("User Authenticated: " + resultResponse.userId);
            FetchUsername();
            onVerified?.Invoke();
        }

        private void OnFailedVerification()
        {
            FractalUtils.Log("Session verification failed or expired.");
            authUserText.text = FractalConstants.ButtonLogin;
            onError?.Invoke();
        }

        private async void FetchUsername()
        {
            try { 
                UserInfo userInfo = await FractalClient.Instance.GetUser();
                authUserText.text = userInfo.username;
            }
            catch
            {
                throw new FractalInvalidResponse();
            }
        }
    }
}
