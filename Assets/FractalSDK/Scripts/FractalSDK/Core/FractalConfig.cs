using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FractalSDK.Core
{
    [CreateAssetMenu(fileName = "FractalConfig", menuName = "Fractal/Config", order = 1)]

    public class FractalConfig : ScriptableObject

    {
        public string clientId = "YOUR_CLIENT_ID_GOES HERE";
    }
}
