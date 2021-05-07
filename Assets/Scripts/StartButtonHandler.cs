using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButtonHandler : MonoBehaviour
{
    public void SetText(string text)
    {
        Text txt = transform.Find("Text").GetComponent<Text>();
        txt.text = text;
    }

    public void NextScene(int index)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PrevScene(int index)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }


}
