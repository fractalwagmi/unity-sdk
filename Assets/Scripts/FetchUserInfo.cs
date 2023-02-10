using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FetchUserInfo : MonoBehaviour
{
    public TextMeshProUGUI userMail;
    public TextMeshProUGUI userPublicKey;
    public TextMeshProUGUI userName;

    public void GetFractalUserData()
    {
        FractalClient.Instance.GetUser(userData =>
        {
            userMail.text = userData.email;
            userPublicKey.text = userData.accountPublicKey;
            userName.text = userData.username;
        });
    }
}
