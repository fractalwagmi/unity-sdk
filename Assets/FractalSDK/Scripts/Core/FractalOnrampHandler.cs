using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FractalSDK.Models.Api;
using UnityEngine;
using UnityEngine.UI;

namespace FractalSDK.Core
{
    public class FractalOnrampHandler : MonoBehaviour
    {
        #region Fields

        [Tooltip("Button object to manually initiate the onramp process.")]
        public Button onrampButton;

        [Tooltip("Game clientId you can obtain from Fstudio.")]
        public string clientId;

        #endregion

        #region Externals

#if !UNITY_SWITCH && !UNITY_PS4 && !UNITY_PS4 && !UNITY_XBOXONE

        //[External JS Call]
        [DllImport("__Internal")]
        private static extern void OpenFractalPopup(string url);

#else
        private static void OpenFractalPopup(string url)
        {
            throw new NotImplementedException();
        }


#endif
        #endregion

        void Start()
        {
            Button onrampPrefab = onrampButton.GetComponent<Button>();
            onrampPrefab.onClick.AddListener(OpenOnramp);
        }


        /// <summary>
        /// Opens Fractal authentication URL in the systems default browser.
        /// On WebGL a Fractal plugin is used to open the authentication in popup.
        /// </summary>
        /// <param name="authUrl">Authentication URL to open.</param>
        private void OpenOnramp()
        {
            string baseUrl= "https://www.fractal.is/onramp?clientId=";

            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                    OpenFractalPopup(baseUrl + clientId);
                    break;

                default:
                    Application.OpenURL(baseUrl + clientId);
                    break;
            }

        }

    }
}
