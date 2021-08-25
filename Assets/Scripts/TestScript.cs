using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Button signup;
    // Start is called before the first frame update
    void Start()
    {
        signup.onClick.AddListener(() => redirect());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void redirect()
    {
        SceneManager.LoadScene("Profile");
    }
}
