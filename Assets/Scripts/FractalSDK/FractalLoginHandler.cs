using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;

public class FractalLoginHandler : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void OpenBrowserPopup(string url);

	[Tooltip("Game will try to automatically authenticate the player from supplied query token.")]
	public bool autoLogin = true;

	[Tooltip("Button object to manualy initiate the login process.")]
	public Button loginButton;

	[Tooltip("Game clientId you can obtain from Fstudio.")]
	public string clientId;

	[Tooltip("Specify the scopes that the game will request from the API.")]
	public Scope[] scopes = { Scope.IDENTIFY };

	[Tooltip("Event to call when user has begin authentication.")]
	public UnityEvent onStarted;

	[Tooltip("Event to call when user is sucessfully signedin.")]
	public UnityEvent onVerified;

	[Tooltip("Event to call when authentication failed and expired.")]
	public UnityEvent onError;

	private IEnumerator poolingCoroutine;

	private const int LOGIN_POOLING_PERIOD = 10;


	void Start()
	{
		Button loginPrefab = loginButton.GetComponent<Button>();
		loginPrefab.onClick.AddListener(InitAuth);
		FractalClient.Instance.Init(clientId, scopes);
	}


	/// <summary>
	/// Initializes Fractal authentication process. 
	/// </summary>
	public void InitAuth()
	{
		onStarted?.Invoke();
		FractalClient.Instance.GetAuthUrl(OpenAuth);
	}

	/// <summary>
	/// Opens Fractal authentication URL in the systems default browser.
	/// On WebGL a Fractal plugin is used to open the authentication in popup.
	/// </summary>
	/// <param name="authUrl">Authentication URL to open.</param>
	private void OpenAuth(AuthResponse authUrl)
    {
		FractalUtils.Log("Executing user authentication.");

		if (poolingCoroutine != null)
		{
			StopCoroutine(poolingCoroutine);
		}

		switch (Application.platform)
        {
            case RuntimePlatform.WebGLPlayer:
				OpenBrowserPopup(authUrl.url);
				break;

			default:
				Application.OpenURL(authUrl.url);
				break;
		}

		poolingCoroutine = LoginPooler(authUrl.code);
		StartCoroutine(poolingCoroutine);
	}

	/// <summary>
	/// Pools if the user finsihed the authentication in the browser.
	/// </summary>
	/// <param name="code">Authentication code to validate.</param>
	private IEnumerator LoginPooler(string code)
	{
		for (int i = 0; i < LOGIN_POOLING_PERIOD; i++)
		{
			yield return new WaitForSeconds(1f);
			if (Application.isFocused)
			{
				FractalClient.Instance.GetAuthResult(code, OnFinishedVerification);
			}
		}
		OnFailedVerification();
	}

	private void OnFinishedVerification(ResultResponse resultResponse)
	{
		StopCoroutine(poolingCoroutine);
		FractalUtils.Log("User authenticated");
        onVerified?.Invoke();
    }

	private void OnFailedVerification()
	{
		StopCoroutine(poolingCoroutine);
		FractalUtils.Log("User verification failed / expired");
		onError?.Invoke();
	}
}