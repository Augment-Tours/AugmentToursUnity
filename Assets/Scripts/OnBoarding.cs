using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnBoarding : MonoBehaviour
{
    public Button signupButton;
    public Button signinButton;
    // Start is called before the first frame update
    void Start()
    {
        signinButton.onClick.AddListener(() => SigninPressed());
        signupButton.onClick.AddListener(() => SignupPressed());
    }

    void SignupPressed() {
        SceneManager.LoadScene("SignUp");
    }

    void SigninPressed() {
        // Debug.Log("Signin Pressed");
        SceneManager.LoadScene("Signin");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
