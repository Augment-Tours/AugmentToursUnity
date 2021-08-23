using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Firebase.Auth;
using Firebase.Extensions;

public class Logout : MonoBehaviour
{
    public Button logoutButton;
    // Start is called before the first frame update
    void Start()
    {
        logoutButton.onClick.AddListener(() => LogoutPressed());
    }

    void LogoutPressed() {
        // signout from firebase
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log("Logged out " + FirebaseAuth.DefaultInstance.CurrentUser);
        SceneManager.LoadScene("Signin");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
