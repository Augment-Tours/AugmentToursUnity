using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class Navigation : MonoBehaviour
{
    // Start is called before the first frame update
    public Button FavoritesButton;
    public Button HomeButton;
    public Button ProfileButton;
    public Button ArModelButton;
    void Start()
    {
        FavoritesButton.onClick.AddListener(() => FavoritesPressed());
        HomeButton.onClick.AddListener(() => HomePressed());
        ProfileButton.onClick.AddListener(() => ProfilePressed());
        ArModelButton.onClick.AddListener(() => ArModelPressed());
    }
    void FavoritesPressed() {
        loadScene("Favorites");
    }
    void HomePressed() {
        SceneManager.LoadScene("BrowseMuseums");
    }
    void ProfilePressed() {
        loadScene("Profile");
        // Debug.Log("Profile Nav Button Pressed");
    }

    void ArModelPressed(){
        Debug.Log("AR Nav Button Pressed");
        SceneManager.LoadScene("MainScreen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void loadScene(string scene)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if(user == null)
        {
            SceneManager.LoadScene("Signin");
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }
}
