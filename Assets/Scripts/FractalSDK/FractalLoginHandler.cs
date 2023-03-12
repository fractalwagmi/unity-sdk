using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using TMPro;
using System;
using System.Threading.Tasks;
using FractalSDK.Core;
using FractalSDK.Enums;
using FractalSDK.Models.Api;

public class FractalLoginHandler : MonoBehaviour
{
    #region Fields

    [Tooltip("Button object to manualy initiate the login process.")]
    public Button loginButton;

    [Tooltip("Text mesh to show user's account e-mail.")]
    public TextMeshProUGUI authUserText;

    [Tooltip("Game clientId you can obtain from Fstudio.")]
    public string clientId;

    [Tooltip("Specify the scopes that the game will request from the API.")]
    public Scope[] scopes;

    [Tooltip("Event to call when user has begin authentication.")]
    public UnityEvent onStarted;

    [Tooltip("Event to call when user is sucessfully signedin.")]
    public UnityEvent onVerified;

    [Tooltip("Event to call when authentication failed and expired.")]
    public UnityEvent onError;

    #endregion


    #region Externals

    //[External JS Call]
    [DllImport("__Internal")]
    private static extern void SetupFractalEvents();

    //[External JS Call]
    [DllImport("__Internal")]
    private static extern void OpenFractalPopup(string url);

    //[External JS Call]
    [DllImport("__Internal")]
    private static extern void CloseFractalPopup();

    #endregion


    private string _loginCode;


    void Start()
    {
        Button loginPrefab = loginButton.GetComponent<Button>();
        loginPrefab.onClick.AddListener(InitAuth);
        FractalClient.Instance.Init(clientId, scopes);

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
        onStarted?.Invoke();
        try
        {
            AuthResponse authUrl = await FractalClient.Instance.GetAuthUrl();
            OpenAuth(authUrl);
        }
        catch(Exception ex)
        {
            onError?.Invoke();
            Debug.Log(ex);
        }

    }

    /// <summary>
    /// Opens Fractal authentication URL in the systems default browser.
    /// On WebGL a Fractal plugin is used to open the authentication in popup.
    /// </summary>
    /// <param name="authUrl">Authentication URL to open.</param>
    private void OpenAuth(AuthResponse authUrl)
    {
        authUserText.text = FractalConstants.ButtonLoading;
        _loginCode = authUrl.code;

        switch (Application.platform)
        {
            case RuntimePlatform.WebGLPlayer:
                OpenFractalPopup(authUrl.url);
                break;

            default:
                Application.OpenURL(authUrl.url);
                LoginPooler(_loginCode);
                break;
        }

    }

    /// <summary>
    /// Handles the callback messages from the Fractal auth popup (WebGL only)
    /// </summary>
    /// <param name="payload">Message recieved from the WebGL popup.</param>
    public async void HandlePopupMessage(string payload)
    {
        switch (payload){
            case "PROJECT_APPROVED":
                try
                {
                    CloseFractalPopup();
                    ResultResponse result = await FractalClient.Instance.GetAuthResult(_loginCode);
                    OnFinishedVerification(result);
                    break;
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
                    ResultResponse result = await FractalClient.Instance.GetAuthResult(code);
                    OnFinishedVerification(result);
                    return;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }
        OnFailedVerification();
    }

    private async void OnFinishedVerification(ResultResponse resultResponse)
    {
        FractalUtils.Log("User Authenticated: " + resultResponse.userId);

        try
        {
            UserInfo user = await FractalClient.Instance.GetUser();
            authUserText.text = user.username;
        }
        catch (FractalNotAuthenticated)
        {
            Debug.Log("User is not authenticated");
            throw;
        }
        catch (FractalAPIRequestError)
        {
            Debug.Log("Device is offline");
            throw;
        }


        onVerified?.Invoke();
    }

    private void OnFailedVerification()
    {
        FractalUtils.Log("Session verification failed or expired.");
        authUserText.text = FractalConstants.ButtonLogin;
        onError?.Invoke();
    }
}